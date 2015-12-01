# Introduction #

**svn\_spider** is the script used for maintaining the metadata in our
subversion repository.  The script can be used to keep track of
several subversion repositories and set properties on repository files
according to a set of given rules. (e.g., html-files have mime-type
'text/html', and should have keyword substitution for $Rev$ and
$Date$)


# Details #

svn\_spider is run automatically at regular intervals on
hypocrates.peopleware.be, a machine owned by PeopleWare.  This is done
with a crontab entry from the "rvdginste" account.  Cron runs the
svn\_spider script on the following location:
/home/rvdginste/svn\_spider/svn\_spider.rb.

Fixes by svn\_spider in the repository are committed with a developer
account created specifically for this purpose (ppw.bot@gmail.com).

The script is currently configured to check all trunks in the ppwcode
repository.

The configuration file currently in use, is located at
/home/rvdginste/svn\_spider/config.rb.  The following is a snapshot of
this file, which can serve as an example:
```
# working dir for code checkouts
@@working_directory = "/home/rvdginste/spider_workdir"

# credentials
@@credentials = {
  "ppwcode" => {
    "username" => "ppw.bot",
    "password" => "xxxxxxxxxxxx"
  }
}

# repositories to check and fix
@@repositories = {
  "vernacular-persistence" => {
    "url" => "https://ppwcode.googlecode.com/svn/java/vernacular/persistence/trunk",
    "auth" => "ppwcode"
  },
  "vernacular-semantics" => {
    "url" => "https://ppwcode.googlecode.com/svn/java/vernacular/semantics/trunk",
    "auth" => "ppwcode"
  },
  "vernacular-exception" => {
    "url" => "https://ppwcode.googlecode.com/svn/java/vernacular/exception/trunk",
    "auth" => "ppwcode"
  },
  "metainfo" => {
    "url" => "https://ppwcode.googlecode.com/svn/java/metainfo/trunk",
    "auth" => "ppwcode"
  },
  "util-collection" => {
    "url" => "https://ppwcode.googlecode.com/svn/java/util/collections/trunk",
    "auth" => "ppwcode"
  },
  "util-reflection" => {
    "url" => "https://ppwcode.googlecode.com/svn/java/util/reflection/trunk",
    "auth" => "ppwcode"
  },
  "util-smallfries" => {
    "url" => "https://ppwcode.googlecode.com/svn/java/util/smallfries/trunk",
    "auth" => "ppwcode"
  },
  "website" => {
    "url" => "https://ppwcode.googlecode.com/svn/website/trunk",
    "auth" => "ppwcode"
  },
  "svn_spider" => {
    "url" => "https://ppwcode.googlecode.com/svn/svn/svn_spider/trunk",
    "auth" => "ppwcode"
  }
}

# mime types (this will override the mime_registry already included)
@@mime_types = {
  # exact extension and corresponding mime type
  # WARNING: mime-type should be "text/..." for non-binary 
  #   files (otherwise subversion will 'likely' treat it as
  #   a binary file)
  "java" => "text/x-java-source",
  "xml" => "text/xml",
  "rb" => "text/x-ruby-source"
}

# mapping of file patterns to need-to-set properties
@@property_filters = {
  # java files
  /\.java$/ => {
    Svn::Core::PROP_EOL_STYLE => "native",
    Svn::Core::PROP_KEYWORDS  => "Date Rev"
  },
  # properties files
  /\.properties$/ => {
    Svn::Core::PROP_EOL_STYLE => "native"
  },
  # xml files
  /\.xml$/ => {
    Svn::Core::PROP_EOL_STYLE => "native",
    Svn::Core::PROP_KEYWORDS  => "Date Rev Id URL"
  },
  # ruby files
  /\.rb$/ => {
    Svn::Core::PROP_EOL_STYLE => "native",
    Svn::Core::PROP_KEYWORDS  => "Date Rev Id URL",
    Svn::Core::PROP_EXECUTABLE => "ON"
  },
  # html files
  /\.html$/ => {
    Svn::Core::PROP_EOL_STYLE => "native",
    Svn::Core::PROP_KEYWORDS  => "Date"
  },
  # NOTICE
  /NOTICE$/ => {
    Svn::Core::PROP_EOL_STYLE => "native",
    Svn::Core::PROP_KEYWORDS  => "Date Rev URL"
  }
}
```