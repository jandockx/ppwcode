/*
 * Copyright 2004 - $Date: 2008-11-15 23:58:07 +0100 (za, 15 nov 2008) $ by PeopleWare n.v..
 *
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 * http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */

#region Using

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;

using log4net;

using Microsoft.SharePoint.Client;

using PPWCode.Util.SharePoint.I.Helpers;

using Spring.Expressions.Parser.antlr;

using File = Microsoft.SharePoint.Client.File;

#endregion

namespace PPWCode.Util.SharePoint.I
{
    /// <summary>
    /// Actual implementation of <see cref="ISharePointClient"/>.
    /// </summary>
    public class SharePointClient
        : ISharePointClient
    {
        #region fields

        private static readonly ILog s_Logger = LogManager.GetLogger(typeof(SharePointClient));

        #endregion

        #region Private helpers

        /// <summary>
        /// Gets a client context on which the rootweb is already loaded.
        /// User is responsible for disposing the context.
        /// </summary>
        private ClientContext GetSharePointClientContext()
        {
            ClientContext ctx = new ClientContext(SharePointSiteUrl);

            if (Credentials != null)
            {
                s_Logger.Debug("GetSharePointClientContext: Credentials ok");
                ctx.Credentials = Credentials;
            }
            else
            {
                s_Logger.Debug("GetSharePointClientContext: Credentials not ok");
            }

            ctx.Load(ctx.Site.RootWeb);
            ctx.ExecuteQuery();
            s_Logger.Debug(string.Format("Connect to SharePoint using user {0}", ctx.Web.CurrentUser));
            return ctx;
        }

        private static void CreateFolderForEnsure(ClientContext spClientContext, string relativeUrl)
        {
            string workUrl = relativeUrl.StartsWith("/")
                                 ? relativeUrl.Substring(1)
                                 : relativeUrl;
            string[] foldernames = workUrl.Split('/');

            spClientContext.Load(spClientContext.Site.RootWeb.RootFolder);
            spClientContext.ExecuteQuery();

            Folder parentfolder = spClientContext.Site.RootWeb.RootFolder;

            string workname = String.Empty;
            string parentfoldername = String.Empty;
            foreach (string folderName in foldernames)
            {
                try
                {
                    workname = String.Format("{0}/{1}", workname, folderName);
                    Folder workfolder = spClientContext.Site.RootWeb.GetFolderByServerRelativeUrl(workname);
                    spClientContext.Load(workfolder);
                    spClientContext.ExecuteQuery();

                    parentfolder = workfolder;
                }
                catch (ServerException se)
                {
                    if (se.ServerErrorTypeName == typeof(FileNotFoundException).FullName)
                    {
                        if (parentfolder == null)
                        {
                            parentfolder = spClientContext.Site.RootWeb.GetFolderByServerRelativeUrl(parentfoldername);
                            spClientContext.Load(parentfolder);
                        }
                        parentfolder.Folders.Add(folderName);
                        spClientContext.ExecuteQuery();
                        parentfolder = null;
                    }
                }
                parentfoldername = workname;
            }
        }

        #endregion

        #region ISharePointClient interface

        public string SharePointSiteUrl { get; set; }

        public ICredentials Credentials { get; set; }

        /// <inheritdoc cref="ISharePointClient.EnsureFolder" />
        public void EnsureFolder(string relativeUrl)
        {
            try
            {
                using (ClientContext spClientContext = GetSharePointClientContext())
                {
                    Web rootWeb = spClientContext.Site.RootWeb;

                    //Check if the url exists
                    try
                    {
                        rootWeb.GetFolderByServerRelativeUrl(relativeUrl);
                        spClientContext.ExecuteQuery();
                    }
                    catch (ServerException se)
                    {
                        // If not, create it.
                        if (se.ServerErrorTypeName == typeof(FileNotFoundException).FullName)
                        {
                            CreateFolderForEnsure(spClientContext, relativeUrl);
                        }
                    }
                }
            }
            catch (Exception e)
            {
                s_Logger.Error(string.Format("EnsureFolder({0}) failed using ClientContext {1}.", relativeUrl, SharePointSiteUrl), e);
                throw;
            }
        }

        public SharePointDocument DownloadDocument(string relativeUrl)
        {
            try
            {
                using (ClientContext spClientContext = GetSharePointClientContext())
                {
                    Web currentWeb = spClientContext.Web;

                    spClientContext.Load(currentWeb);
                    spClientContext.ExecuteQuery();

                    var fi = File.OpenBinaryDirect(spClientContext, relativeUrl);

                    if (fi == null)
                    {
                        throw new ApplicationException(@"fi == null");
                    }

                    if (fi.Stream == null)
                    {
                        throw new ApplicationException(@"fi.Stream == null");
                    }
                    return new SharePointDocument(fi.Stream.ConvertToByteArray());
                }
            }
            catch (Exception e)
            {
                s_Logger.Error(string.Format("DownloadDocument({0}) failed using ClientContext {1}.", relativeUrl, SharePointSiteUrl), e);
                throw;
            }
        }

        public SharePointDocument DownloadSpecificVersion(string baseRelativeUrl, string version)
        {
            try
            {
                using (ClientContext spClientContext = GetSharePointClientContext())
                {
                    Web currentWeb = spClientContext.Web;

                    spClientContext.Load(currentWeb);
                    spClientContext.ExecuteQuery();

                    List<SharePointDocumentVersion> fileVersionCollection =
                        RetrieveAllVersionsFromUrl(baseRelativeUrl)
                            .OrderBy(x => x.CreationDate)
                            .ToList();

                    SharePointDocumentVersion specificVersion =
                        fileVersionCollection.SingleOrDefault(x => x.Version == version);

                    // nothing found: throw semantic exception
                    if (specificVersion == null)
                    {
                        throw new SemanticException("VERSION_DOES_NOT_EXIST");
                    }

                    // current?
                    if (specificVersion.Version == fileVersionCollection.Last().Version)
                    {
                        return DownloadDocument(baseRelativeUrl);
                    }

                    // older version
                    var wc = new WebClient()
                    {
                        UseDefaultCredentials = true
                    };
                    string fileVerUrl = spClientContext.Url + "/" + specificVersion.Url;
                    byte[] content = wc.DownloadData(fileVerUrl);
                    return new SharePointDocument(content);
                }
            }
            catch (Exception e)
            {
                s_Logger.Error(string.Format("DownloadDocument({0}) failed using ClientContext {1}.", baseRelativeUrl, SharePointSiteUrl), e);
                throw;
            }
        }

        public void UploadDocument(string relativeUrl, SharePointDocument doc)
        {
            UploadDocumentRetrieveVersion(relativeUrl, doc, false);
        }

        public string UploadDocumentReceiveVersion(string relativeUrl, SharePointDocument doc)
        {
            return UploadDocumentRetrieveVersion(relativeUrl, doc, true);
        }

        #region private helpers for upload

        private string UploadDocumentRetrieveVersion(string relativeUrl, SharePointDocument doc, bool retrieveVersion)
        {
            using (ClientContext spClientContext = GetSharePointClientContext())
            {
                //Check if the url exists
                int index = relativeUrl.LastIndexOf("/");
                string parentFolder = relativeUrl.Substring(0, index);

                //Create intermediate folders if not exist
                EnsureFolder(parentFolder);
                spClientContext.ExecuteQuery();

                string result = null;

                try
                {
                    Upload(relativeUrl, doc);
                    if (retrieveVersion)
                    {
                        SharePointDocumentVersion sharePointDocumentVersion = RetrieveCurrentVersion(relativeUrl, spClientContext);
                        result = sharePointDocumentVersion.Version;
                    }
                }
                catch (Exception e)
                {
                    s_Logger.Error(string.Format("UploadDocument({0}) failed using ClientContext {1}.", relativeUrl, SharePointSiteUrl), e);
                    throw;
                }

                return result;
            }
        }

        private void Upload(string relativeUrl, SharePointDocument doc)
        {
            string targetUrl = string.Format("{0}{1}", SharePointSiteUrl, relativeUrl);

            // Create a PUT Web request to upload the file.
            WebRequest request = WebRequest.Create(targetUrl);

            //Set credentials of the current security context
            request.Credentials = CredentialCache.DefaultCredentials;
            request.Method = "PUT";

            // Create buffer to transfer file
            byte[] fileBuffer = new byte[1024];

            // Write the contents of the local file to the request stream.
            using (Stream stream = request.GetRequestStream())
            {
                //Load the content from local file to stream
                using (MemoryStream ms = new MemoryStream(doc.Content))
                {
                    ms.Position = 0;

                    // Get the start point
                    int startBuffer = ms.Read(fileBuffer, 0, fileBuffer.Length);
                    for (int i = startBuffer; i > 0; i = ms.Read(fileBuffer, 0, fileBuffer.Length))
                    {
                        stream.Write(fileBuffer, 0, i);
                    }
                }
            }

            // Perform the PUT request
            WebResponse response = request.GetResponse();
            if (response != null)
            {
                //Close response
                response.Close();
            }
        }

        #endregion

        public bool ValidateUri(Uri sharePointUri)
        {
            if (sharePointUri != null)
            {
                string baseUrl = sharePointUri.GetLeftPart(UriPartial.Authority);

                if (!string.IsNullOrEmpty(baseUrl))
                {
                    using (ClientContext clientContext = new ClientContext(baseUrl))
                    {
                        if (Credentials != null)
                        {
                            clientContext.Credentials = Credentials;
                        }

                        //get the site collection
                        Web site = clientContext.Web;

                        string localPath = sharePointUri.LocalPath;

                        if (!string.IsNullOrEmpty(localPath))
                        {
                            //get the document library folder
                            site.GetFolderByServerRelativeUrl(localPath);
                            try
                            {
                                clientContext.ExecuteQuery();
                                return true;
                            }
                            catch (Exception)
                            {
                                return false;
                            }
                        }
                    }
                }
            }
            return false;
        }

        public void OpenUri(Uri uri)
        {
            string url = uri.OriginalString;
            if (!string.IsNullOrEmpty(url))
            {
                Process.Start(new ProcessStartInfo
                {
                    UseShellExecute = true,
                    FileName = url,
                    Verb = "Open",
                    LoadUserProfile = true
                });
            }
        }

        public List<SharePointSearchResult> SearchFiles(string url)
        {
            try
            {
                using (ClientContext spClientContext = GetSharePointClientContext())
                {
                    Web rootWeb = spClientContext.Site.RootWeb;
                    Folder spFolder = rootWeb.GetFolderByServerRelativeUrl(url); // "/Shared%20Documents/MEERTENS%20MATHIEU%20-%2051062002701/Construo/Actua/");
                    spClientContext.Load(spFolder.Files);
                    spClientContext.ExecuteQuery();

                    List<SharePointSearchResult> result = new List<SharePointSearchResult>();
                    foreach (File spFile in spFolder.Files)
                    {
                        var fileInformation = new SharePointSearchResult();
                        fileInformation.Properties.Add("FileName", spFile.Name);
                        fileInformation.Properties.Add("Description", spFile.CheckInComment);
                        fileInformation.Properties.Add("MajorVersion", spFile.MajorVersion);
                        fileInformation.Properties.Add("MinorVersion", spFile.MinorVersion);
                        fileInformation.Properties.Add("ModifiedBy", spFile.ModifiedBy);
                        fileInformation.Properties.Add("DateModified", spFile.TimeLastModified);
                        fileInformation.Properties.Add("CreatedBy", spFile.Author);
                        fileInformation.Properties.Add("DateCreated", spFile.TimeCreated);
                        fileInformation.Properties.Add("ServerRelativeUrl", spFile.ServerRelativeUrl);
                        result.Add(fileInformation);
                    }
                    return result;
                }
            }
            catch (Exception e)
            {
                s_Logger.Error(string.Format("SearchFiles({0}) failed using ClientContext {1}.", url, SharePointSiteUrl), e);
                throw;
            }
        }

        // renameFolder(string baseRelativeUrl, string oldFolderName, string newFolderName)
        // ex. renameFolder("/PensioB", "NAME, FIRSTNAME@123409876@12", "NAME, FIRSTNAME@123409876@9876")
        // ex. renameFolder("/PensioB/NAME, FIRSTNAME@123409876@12/Payments/Beneficiaries", "NAME, FIRSTNAME@123409876@12", "NAME, FIRSTNAME@123409876@9876")
        // list /PensioB
        // --> listitem NAME, FIRSTNAME@123409876@12
        //   --> list
        //     --> listitem Payments
        // ....
        // ("PensioB", "AAA-Test/test1/test2", "AAA-Test1/test3/test9")
        public void RenameFolder(string urlContainingFolder, string oldFolderName, string newFolderName)
        {
            try
            {
                using (ClientContext spClientContext = GetSharePointClientContext())
                {
                    Web web = spClientContext.Site.RootWeb;

                    // get document library
                    List list = web.Lists.GetByTitle(ExtractListName(urlContainingFolder));

                    // find all items with given name inside the baseRelativeUrl
                    CamlQuery query = CreateCamlQueryFindExactFolderPath(
                        urlContainingFolder,
                        string.Format("{0}/{1}", urlContainingFolder, oldFolderName));
                    ListItemCollection listItemCollection = list.GetItems(query);

                    // To load all fields, take the following
                    // spClientContext.Load(listItemCollection);

                    // only load required fields
                    spClientContext.Load(
                        listItemCollection,
                        fs => fs.Include(
                            fi => fi["Title"],
                            fi => fi["FileLeafRef"],
                            fi => fi["FileRef"]));
                    spClientContext.ExecuteQuery();

                    // for all found folders, rename them
                    if (listItemCollection.Count != 0)
                    {
                        ListItem listitem = listItemCollection[0];
                        s_Logger.DebugFormat("Title:       {0}", listitem["Title"]);
                        s_Logger.DebugFormat("FileLeafRef: {0}", listitem["FileLeafRef"]);
                        s_Logger.DebugFormat("FileRef:     {0}", listitem["FileRef"]);
                        listitem["Title"] = newFolderName;
                        listitem["FileLeafRef"] = newFolderName;
                        listitem.Update();
                        spClientContext.ExecuteQuery();
                    }
                }
            }
            catch (Exception e)
            {
                s_Logger.ErrorFormat(
                    "Error renaming in [{0}] from old name [{1}] to new name [{2}]. Exception({3}).",
                    urlContainingFolder,
                    oldFolderName,
                    newFolderName,
                    e);
                throw;
            }
        }

        #region private helper methods

        private static string ExtractListName(string relativeUrl)
        {
            string listBase = relativeUrl;
            if (listBase.StartsWith("/"))
            {
                listBase = listBase.Remove(0, 1);
            }
            listBase = listBase.Split('/')[0];
            return listBase;
        }

        #endregion

        #region Caml queries

        // ReSharper disable MemberCanBeMadeStatic.Local
        private CamlQuery CreateCamlQueryFindExactFolderPath(string baseRelativeUrl, string oldFolderName)
            // ReSharper restore MemberCanBeMadeStatic.Local
        {
            CamlQuery query = new CamlQuery();
            query.ViewXml = "<View Scope=\"RecursiveAll\"> " +
                            "<Query>" +
                            "<Where>" +
                            "<And>" +
                            "<Eq>" +
                            "<FieldRef Name=\"FSObjType\" />" +
                            "<Value Type=\"Integer\">1</Value>" +
                            "</Eq>" +
                            "<Eq>" +
                            "<FieldRef Name=\"FileRef\"/>" +
                            "<Value Type=\"Text\">" + oldFolderName + "</Value>" +
                            "</Eq>" +
                            "</And>" +
                            "</Where>" +
                            "</Query>" +
                            "</View>";
            query.FolderServerRelativeUrl = baseRelativeUrl;
            return query;
        }

        // ReSharper disable MemberCanBeMadeStatic.Local
        private CamlQuery CreateCamlQueryFindAllOccurencesOfFolder(string baseRelativeUrl, string oldFolderName)
            // ReSharper restore MemberCanBeMadeStatic.Local
        {
            CamlQuery query = new CamlQuery();
            query.ViewXml = "<View Scope=\"RecursiveAll\"> " +
                            "<Query>" +
                            "<Where>" +
                            "<And>" +
                            "<Eq>" +
                            "<FieldRef Name=\"FSObjType\" />" +
                            "<Value Type=\"Integer\">1</Value>" +
                            "</Eq>" +
                            "<Eq>" +
                            "<FieldRef Name=\"FileLeafRef\"/>" +
                            "<Value Type=\"Text\">" + oldFolderName + "</Value>" +
                            "</Eq>" +
                            "</And>" +
                            "</Where>" +
                            "</Query>" +
                            "</View>";
            query.FolderServerRelativeUrl = baseRelativeUrl;
            return query;
        }

        #endregion

        // renameFolder(string baseRelativeUrl, string oldFolderName, string newFolderName)
        // ex. renameAllOccurencesOfFolder("/PensioB", "NAME, FIRSTNAME@123409876@12", "NAME, FIRSTNAME@123409876@9876")
        // ex. renameAllOccurencesOfFolder("/PensioB/NAME, FIRSTNAME@123409876@12/Payments/Beneficiaries", "NAME, FIRSTNAME@123409876@12", "NAME, FIRSTNAME@123409876@9876")
        // list /PensioB
        // --> listitem NAME, FIRSTNAME@123409876@12
        //   --> list
        //     --> listitem Payments
        // ....
        // ("PensioB", "AAA-Test/test1/test2", "AAA-Test1/test3/test9")
        public void RenameAllOccurrencesOfFolder(string baseRelativeUrl, string oldFolderName, string newFolderName)
        {
            List<string> renamedListItemCollection = new List<string>();
            try
            {
                using (ClientContext spClientContext = GetSharePointClientContext())
                {
                    Web web = spClientContext.Site.RootWeb;

                    // get document library
                    List list = web.Lists.GetByTitle(ExtractListName(baseRelativeUrl));

                    // find all items with given name inside the baseRelativeUrl
                    CamlQuery query = CreateCamlQueryFindAllOccurencesOfFolder(baseRelativeUrl, oldFolderName);
                    ListItemCollection listItemCollection = list.GetItems(query);

                    // To load all fields, take the following
                    // spClientContext.Load(listItemCollection);

                    // only load required fields
                    spClientContext.Load(
                        listItemCollection,
                        fs => fs.Include(
                            fi => fi["Title"],
                            fi => fi["FileLeafRef"],
                            fi => fi["FileRef"]));
                    spClientContext.ExecuteQuery();

                    // for all found folders, rename them
                    if (listItemCollection.Count != 0)
                    {
                        for (var counter = 0; counter < listItemCollection.Count; counter++)
                        {
                            s_Logger.DebugFormat("Title:       {0}", listItemCollection[counter]["Title"]);
                            s_Logger.DebugFormat("FileLeafRef: {0}", listItemCollection[counter]["FileLeafRef"]);
                            s_Logger.DebugFormat("FileRef:     {0}", listItemCollection[counter]["FileRef"]);
                            listItemCollection[counter]["Title"] = newFolderName;
                            listItemCollection[counter]["FileLeafRef"] = newFolderName;
                            listItemCollection[counter].Update();
                            spClientContext.ExecuteQuery();
                            string newFileRef = listItemCollection[counter]["FileRef"].ToString();
                            renamedListItemCollection.Add(newFileRef);
                        }
                    }
                }
            }
            catch (Exception e)
            {
                renamedListItemCollection.Reverse();
                foreach (string item in renamedListItemCollection)
                {
                    string relativeUrl = ExtractRelativeUrlFromBaseRelativeUrl(item);
                    try
                    {
                        RenameFolder(relativeUrl, newFolderName, oldFolderName);
                    }
                    catch (Exception exception)
                    {
                        s_Logger.ErrorFormat(
                            "Error during cleanup (folder={0}, old={1}, new={2}) of failed rename. Exception({3}).",
                            relativeUrl,
                            newFolderName,
                            oldFolderName,
                            exception);
                    }
                }

                s_Logger.ErrorFormat(
                    "Error renaming in [{0}] from old name [{1}] to new name [{2}]. Exception({3}).",
                    baseRelativeUrl,
                    oldFolderName,
                    newFolderName,
                    e);
                throw;
            }
        }

        #endregion

        //create folder
        //if parameter createAll is true
        //the whole path of newFolderName that does not exist, will be created 
        //if parameter createAll is false
        //only the last path in parameter newFolderName will be created if the the rest of the path already exist
        public void CreateFolder(string newFolderName, bool createAll)
        {
            try
            {
                using (ClientContext spClientContext = GetSharePointClientContext())
                {
                    string theNewFolderName = newFolderName;
                    if (!newFolderName.StartsWith("/"))
                    {
                        newFolderName = '/' + newFolderName;
                    }
                    string[] foldernames = newFolderName.Split('/');

                    if (string.IsNullOrEmpty(foldernames[foldernames.Length - 1]))
                    {
                        newFolderName = ExtractRelativeUrlFromBaseRelativeUrl(newFolderName);
                        foldernames = newFolderName.Split('/');
                    }
                    string listName = foldernames[1];

                    if ((foldernames.Length < 3) || ((foldernames.Length == 3) && string.IsNullOrEmpty(foldernames[2])))
                    {
                        string errorInformation = string.Format("Path [{0}] is not valid", theNewFolderName);
                        throw new Exception(string.Format("Error in creating form [{0}].Exeption({1})", theNewFolderName, errorInformation));
                    }
                    if (CheckExistenceOfFolderWithExactPath(newFolderName))
                    {
                        string errorInformation = string.Format("Path [{0}] already exist", theNewFolderName);
                        throw new Exception(string.Format("Error in creating form [{0}].Exeption({1})", theNewFolderName, errorInformation));
                    }

                    Web web = spClientContext.Web;
                    List list = web.Lists.GetByTitle(ExtractListName(newFolderName));

                    if (newFolderName != string.Empty)
                    {
                        if (createAll)
                        {
                            string url = listName;
                            for (int teller = 2; teller < foldernames.Length; teller++)
                            {
                                string folderName = foldernames[teller].Trim();
                                if (folderName != foldernames[teller] || folderName == string.Empty)
                                {
                                    string errorInformation = string.Format("Path [{0}] is not valid", theNewFolderName);
                                    throw new Exception(string.Format("Error in creating form [{0}].Exeption({1})", theNewFolderName, errorInformation));
                                }
                                url += "/" + foldernames[teller];
                                if (!CheckExistenceOfFolderWithExactPath(url))
                                {
                                    string relativeUrl = ExtractRelativeUrlFromBaseRelativeUrl(url);
                                    Create(list, foldernames, teller, relativeUrl, theNewFolderName);
                                }
                            }
                        }
                        else
                        {
                            string url = ExtractRelativeUrlFromBaseRelativeUrl(newFolderName);
                            if (foldernames.Length == 3)
                            {
                                Create(list, foldernames, 2, url, theNewFolderName);
                            }
                            else
                            {
                                if (CheckExistenceOfFolderWithExactPath(url))
                                {
                                    Create(list, foldernames, foldernames.Length - 1, url, theNewFolderName);
                                }
                                else
                                {
                                    string errorInformation = string.Format("Path [{0}] does not exist", url);
                                    throw new Exception(string.Format("Error in creating form [{0}].Exeption({1})", theNewFolderName, errorInformation));
                                }
                            }
                        }
                    }
                    spClientContext.ExecuteQuery();
                }
            }
            catch (Exception ex)
            {
                s_Logger.ErrorFormat(
                    "Error creating folder [{0}]. Exception({1}).",
                    newFolderName,
                    ex);
                throw;
            }
        }

        private void Create(List list, string[] foldernames, int teller, string url, string newFolderName)
        {
            ListItemCreationInformation newItem = new ListItemCreationInformation();

            newItem.UnderlyingObjectType = FileSystemObjectType.Folder;
            newItem.FolderUrl = url;
            string folderName = foldernames[teller].Trim();
            if (folderName != foldernames[teller] || folderName == string.Empty)
            {
                string errorInformation = string.Format("Path [{0}] is not valid", newFolderName);
                throw new Exception(string.Format("Error in creating form [{0}].Exeption({1})", newFolderName, errorInformation));
            }
            newItem.LeafName = foldernames[teller];
            ListItem item = list.AddItem(newItem);
            item["Title"] = foldernames[teller];
            item.Update();
        }

        //delete folder 
        //if parameter deleteChildren is true
        //all subfolders will be deleted
        //if parameter deleteChildren is false
        //folder will only by deleted if folder has no children
        public void DeleteFolder(string folderNameToDelete, bool deleteChildren)
        {
            try
            {
                using (ClientContext spClientcontext = GetSharePointClientContext())
                {
                    string theFolderNameToDelete = folderNameToDelete;
                    // make sure url starts with "/"
                    if (!folderNameToDelete.StartsWith("/"))
                    {
                        folderNameToDelete = '/' + folderNameToDelete;
                    }

                    string[] foldernames = folderNameToDelete.Split('/');

                    if (string.IsNullOrEmpty(foldernames[foldernames.Length - 1]))
                    {
                        folderNameToDelete = ExtractRelativeUrlFromBaseRelativeUrl(folderNameToDelete);
                    }
                    if (!CheckExistenceOfFolderWithExactPath(folderNameToDelete))
                    {
                        string errorInformation = string.Format("Path ({0}) does not exist or can not be deleted", theFolderNameToDelete);
                        throw new Exception(string.Format("Error in deleting form [{0}].Exeption[{1}]", theFolderNameToDelete, errorInformation));
                    }
                    string relativeUrl = ExtractRelativeUrlFromBaseRelativeUrl(folderNameToDelete);

                    Web web = spClientcontext.Web;
                    List list = web.Lists.GetByTitle(ExtractListName(folderNameToDelete));

                    CamlQuery query = CreateCamlQueryFindExactFolderPath(relativeUrl, folderNameToDelete);

                    ListItemCollection listItemCollection = list.GetItems(query);

                    spClientcontext.Load(list);
                    spClientcontext.Load(listItemCollection);
                    spClientcontext.ExecuteQuery();

                    if (deleteChildren)
                    {
                        if (listItemCollection.Count != 0)
                        {
                            foreach (var listitem in listItemCollection)
                            {
                                listitem.DeleteObject();
                            }
                            spClientcontext.ExecuteQuery();
                        }
                        else
                        {
                            string errorInformation = string.Format("Folder to delete does not exist");
                            throw new Exception(string.Format("Error in deleting form [{0}].Exeption[{1}]", theFolderNameToDelete, errorInformation));
                        }
                    }
                    else
                    {
                        Folder folderToDelete = spClientcontext.Site.RootWeb.GetFolderByServerRelativeUrl(folderNameToDelete);
                        spClientcontext.Load(folderToDelete.Files);
                        spClientcontext.Load(folderToDelete.Folders);
                        spClientcontext.ExecuteQuery();
                        if (folderToDelete.Files.Count != 0 || folderToDelete.Folders.Count != 0)
                        {
                            string errorInformation = string.Format("Folder has children");
                            throw new Exception(string.Format("Error in deleting form [{0}].Exeption[{1}]", theFolderNameToDelete, errorInformation));
                        }
                        if (listItemCollection.Count != 0)
                        {
                            foreach (var listitem in listItemCollection)
                            {
                                listitem.DeleteObject();
                            }
                            spClientcontext.ExecuteQuery();
                        }
                        else
                        {
                            string errorInformation = string.Format("Folder to delete does not exist");
                            throw new Exception(string.Format("Error in deleting form [{0}].Exeption[{1}]", theFolderNameToDelete, errorInformation));
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                s_Logger.ErrorFormat(
                    "Error deleting folder [{0}]. Exception({1}).",
                    folderNameToDelete,
                    ex);
                throw;
            }
        }

        //checks if folder exists in list
        //Parameter baseRelativeUrl has to start with List ex.PensioB/test1
        public int CountAllOccurencesOfFolderInPath(string baseRelativeUrl, string foldername)
        {
            using (ClientContext spClientcontext = GetSharePointClientContext())
            {
                if (!string.IsNullOrEmpty(baseRelativeUrl))
                {
                    // make sure url starts with "/"
                    if (!baseRelativeUrl.StartsWith("/"))
                    {
                        baseRelativeUrl = '/' + baseRelativeUrl;
                    }
                    string[] foldernames = baseRelativeUrl.Split('/');

                    //get foldername
                    //string folderName = foldernames[foldernames.Length - 1];

                    //make relativeUrl
                    string relativeUrl = '/' + foldernames[1];
                    if (foldernames.Length > 2)
                    {
                        for (int teller = 2; teller <= foldernames.Length - 1; teller++)
                        {
                            relativeUrl += '/' + foldernames[teller];
                        }
                    }
                    try
                    {
                        Web web = spClientcontext.Web;
                        //get document library
                        List list = web.Lists.GetByTitle(ExtractListName(baseRelativeUrl));
                        CamlQuery query = CreateCamlQueryFindAllOccurencesOfFolder(relativeUrl, foldername);

                        ListItemCollection listItemCollection = list.GetItems(query);
                        spClientcontext.Load(list);
                        spClientcontext.Load(listItemCollection);
                        spClientcontext.ExecuteQuery();

                        return listItemCollection.Count;
                    }
                    catch (Exception ex)
                    {
                        s_Logger.ErrorFormat(
                            "Error searching folder [{0}]. Exception({1}).",
                            baseRelativeUrl,
                            ex);
                        throw;
                    }
                }
            }
            return 0;
        }

        //checks if folder exists in certain path in list
        //parameter baseRelativeUrl has to start with list ex.PensioB/test1
        public bool CheckExistenceOfFolderWithExactPath(string baseRelativeUrl)
        {
            using (ClientContext spClientcontext = GetSharePointClientContext())
            {
                if (!string.IsNullOrEmpty(baseRelativeUrl))
                {
                    // make sure url starts with "/"
                    if (!baseRelativeUrl.StartsWith("/"))
                    {
                        baseRelativeUrl = '/' + baseRelativeUrl;
                    }

                    string[] foldernames = baseRelativeUrl.Split('/');

                    if (string.IsNullOrEmpty(foldernames[foldernames.Length - 1]))
                    {
                        baseRelativeUrl = ExtractRelativeUrlFromBaseRelativeUrl(baseRelativeUrl);
                    }
                    string relativeUrl = ExtractRelativeUrlFromBaseRelativeUrl(baseRelativeUrl);

                    try
                    {
                        //get document library
                        Web web = spClientcontext.Web;
                        List list = web.Lists.GetByTitle(ExtractListName(baseRelativeUrl));
                        CamlQuery query = CreateCamlQueryFindExactFolderPath(relativeUrl, baseRelativeUrl);
                        ListItemCollection listItemCollection = list.GetItems(query);
                        spClientcontext.Load(list);
                        spClientcontext.Load(listItemCollection);
                        spClientcontext.ExecuteQuery();
                        return listItemCollection.Count != 0;
                    }
                    catch (Exception ex)
                    {
                        s_Logger.ErrorFormat(
                            "Error searching folder [{0}]. Exception({1}).",
                            baseRelativeUrl,
                            ex);
                        throw;
                    }
                }
            }
            return false;
        }

        private string ExtractRelativeUrlFromBaseRelativeUrl(string baseRelativeUrl)
        {
            // make sure url starts with "/"
            if (!baseRelativeUrl.StartsWith("/"))
            {
                baseRelativeUrl = '/' + baseRelativeUrl;
            }
            string[] foldernames = baseRelativeUrl.Split('/');
            string relativeUrl = '/' + foldernames[1];

            if (foldernames.Length > 2)
            {
                for (int teller = 2; teller < foldernames.Length - 1; teller++)
                {
                    relativeUrl += '/' + foldernames[teller];
                }
            }
            return relativeUrl;
        }

        public IEnumerable<SharePointDocumentVersion> RetrieveAllVersionsFromUrl(string relativeUrl)
        {
           
            List<SharePointDocumentVersion> documentVersions = new List<SharePointDocumentVersion>();
            
            using (ClientContext spClientcontext = GetSharePointClientContext())
            {
                try
                {
                    File file = spClientcontext.Site.RootWeb.GetFileByServerRelativeUrl(relativeUrl);
                    spClientcontext.Load(file);
                    spClientcontext.Load(file.Versions);
                    spClientcontext.ExecuteQuery();
                    FileVersionCollection collection = file.Versions;
                    foreach (var version in collection)
                    {
                        documentVersions.Add(new SharePointDocumentVersion(version.VersionLabel, version.Created, version.Url));
                    }
                    documentVersions.Add(new SharePointDocumentVersion(file.UIVersionLabel, file.TimeLastModified, file.ServerRelativeUrl));
                    return documentVersions;
                }
                catch (Exception ex)
                {
                    s_Logger.ErrorFormat(
                        "Error searching Versions from file [{0}]. Exception({1}).",
                        relativeUrl,
                        ex);
                    throw;
                }
            }
        }

       private static SharePointDocumentVersion RetrieveCurrentVersion(string relativeUrl, ClientContext spClientcontext)
        {
            try
            {
                File file = spClientcontext.Site.RootWeb.GetFileByServerRelativeUrl(relativeUrl);
                spClientcontext.Load(file);
                spClientcontext.Load(file.Versions);
                spClientcontext.ExecuteQuery();
                return new SharePointDocumentVersion(file.UIVersionLabel, file.TimeLastModified, file.ServerRelativeUrl);
            }
            catch (Exception ex)
            {
                s_Logger.ErrorFormat(
                    "Error searching Major Versions from file [{0}]. Exception({1}).",
                    relativeUrl,
                    ex);
                throw;
            }
        }
    }
}