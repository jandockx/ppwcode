﻿/*
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
using System.Diagnostics.Contracts;
using System.Runtime.Serialization;

using PPWCode.Vernacular.Semantics.I;

#endregion

namespace PPWCode.Vernacular.Persistence.I
{
    [Serializable, DataContract(IsReference = true)]
    public abstract class AbstractPersistentObject :
        AbstractRousseauObject,
        IPersistentObject
    {
        #region Constructor

        protected AbstractPersistentObject()
        {
            Contract.Ensures(PersistenceId == null);
        }

        #endregion

        #region IPersistentObject Members

        [DataMember]
        private long? m_PersistenceId;

        public long? PersistenceId
        {
            get { return m_PersistenceId; }
            set { m_PersistenceId = value; }
        }

        public bool IsTransient
        {
            get { return m_PersistenceId == null; }
        }

        [Pure]
        public bool HasSamePersistenceId(IPersistentObject other)
        {
            return other == null
                       ? false
                       : other.PersistenceId == null || PersistenceId == null
                             ? false
                             : PersistenceId.Value == other.PersistenceId.Value;
        }

        #endregion
    }
}