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
using PPWCode.Vernacular.Exceptions.II;

#endregion

namespace PPWCode.Vernacular.Persistence.II.Dao.NHibernate
{
    [Serializable, DataContract(IsReference = true)]
    public class AuditLog :
        AbstractPersistentObject
    {
        #region constructors

        public AuditLog()
        {
            // base contracts
            // AbstractVersionedPersistentObject
            Contract.Ensures(!PersistenceId.HasValue);

            // This contract
            // association contracts
            Contract.Ensures(EntryType == null);
            Contract.Ensures(!EntityId.HasValue);
            Contract.Ensures(PropertyName == null);
            Contract.Ensures(OldValue == null);
            Contract.Ensures(NewValue == null);
            Contract.Ensures(!CreatedAt.HasValue);
            Contract.Ensures(CreatedBy == null);
        }

        #endregion

        #region Properties

        [DataMember]
        private string m_EntryType;

        public string EntryType
        {
            get
            {
                return m_EntryType;
            }
            set
            {
                Contract.Ensures(EntryType == value);

                if (m_EntryType != value)
                {
                    m_EntryType = value;
                    OnPropertyChanged("EntryType");
                }
            }
        }

        [DataMember]
        private string m_EntityName;

        public string EntityName
        {
            get
            {
                return m_EntityName;
            }
            set
            {
                Contract.Ensures(EntityName == value);

                if (m_EntityName != value)
                {
                    m_EntityName = value;
                    OnPropertyChanged("EntityName");
                }
            }
        }

        [DataMember]
        private long? m_EntityId;

        public long? EntityId
        {
            get
            {
                return m_EntityId;
            }
            set
            {
                Contract.Ensures(EntityId == value);

                if (m_EntityId != value)
                {
                    m_EntityId = value;
                    OnPropertyChanged("EntityId");
                }
            }
        }

        [DataMember]
        private string m_PropertyName;

        public string PropertyName
        {
            get
            {
                return m_PropertyName;
            }
            set
            {
                Contract.Ensures(PropertyName == value);

                if (m_PropertyName != value)
                {
                    m_PropertyName = value;
                    OnPropertyChanged("PropertyName");
                }
            }
        }

        [DataMember]
        private string m_OldValue;

        public string OldValue
        {
            get
            {
                return m_OldValue;
            }
            set
            {
                Contract.Ensures(OldValue == value);

                if (m_OldValue != value)
                {
                    m_OldValue = value;
                    OnPropertyChanged("OldValue");
                }
            }
        }

        [DataMember]
        private string m_NewValue;

        public string NewValue
        {
            get
            {
                return m_NewValue;
            }
            set
            {
                Contract.Ensures(NewValue == value);

                if (m_NewValue != value)
                {
                    m_NewValue = value;
                    OnPropertyChanged("NewValue");
                }
            }
        }

        [DataMember]
        private DateTime? m_CreatedAt;

        public DateTime? CreatedAt
        {
            get
            {
                return m_CreatedAt;
            }
            set
            {
                Contract.Ensures(CreatedAt == value);

                if (m_CreatedAt != value)
                {
                    m_CreatedAt = value;
                    OnPropertyChanged("CreatedAt");
                }
            }
        }

        [DataMember]
        private string m_CreatedBy;

        public string CreatedBy
        {
            get
            {
                return m_CreatedBy;
            }
            set
            {
                Contract.Ensures(CreatedBy == value);

                if (m_CreatedBy != value)
                {
                    m_CreatedBy = value;
                    OnPropertyChanged("CreatedBy");
                }
            }
        }

        #endregion

        #region Civilized tests

        [Pure]
        public override CompoundSemanticException WildExceptions()
        {
#if EXTRA_CONTRACTS
            Contract.Ensures(string.IsNullOrEmpty(EntryType) == Contract.Result<CompoundSemanticException>().ContainsElement(new PropertyException(this, "EntryType", "MANDATORY", null)));
            Contract.Ensures(string.IsNullOrEmpty(EntityName) == Contract.Result<CompoundSemanticException>().ContainsElement(new PropertyException(this, "EntityName", "MANDATORY", null)));
            Contract.Ensures(!EntityId.HasValue == Contract.Result<CompoundSemanticException>().ContainsElement(new PropertyException(this, "EntityId", "MANDATORY", null)));
            Contract.Ensures(string.IsNullOrEmpty(PropertyName) == Contract.Result<CompoundSemanticException>().ContainsElement(new PropertyException(this, "PropertyName", "MANDATORY", null)));
            Contract.Ensures(string.IsNullOrEmpty(OldValue) == Contract.Result<CompoundSemanticException>().ContainsElement(new PropertyException(this, "OldValue", "MANDATORY", null)));
            Contract.Ensures(string.IsNullOrEmpty(NewValue) == Contract.Result<CompoundSemanticException>().ContainsElement(new PropertyException(this, "NewValue", "MANDATORY", null)));
            Contract.Ensures(!CreatedAt.HasValue == Contract.Result<CompoundSemanticException>().ContainsElement(new PropertyException(this, "CreatedAt", "MANDATORY", null)));
            Contract.Ensures(string.IsNullOrEmpty(CreatedBy) == Contract.Result<CompoundSemanticException>().ContainsElement(new PropertyException(this, "CreatedBy", "MANDATORY", null)));
#endif

            CompoundSemanticException cse = base.WildExceptions();
            if (string.IsNullOrEmpty(m_EntryType))
            {
                cse.AddElement(new PropertyException(this, "EntryType", "MANDATORY", null));
            }
            if (string.IsNullOrEmpty(m_EntityName))
            {
                cse.AddElement(new PropertyException(this, "EntityName", "MANDATORY", null));
            }
            if (!m_EntityId.HasValue)
            {
                cse.AddElement(new PropertyException(this, "EntityId", "MANDATORY", null));
            }
            if (string.IsNullOrEmpty(m_PropertyName) && ((m_EntryType == "U") || (m_EntryType == "I")))
            {
                cse.AddElement(new PropertyException(this, "AttributeName", "MANDATORY", null));
            }
            if (string.IsNullOrEmpty(m_OldValue))
            {
                cse.AddElement(new PropertyException(this, "OldValue", "MANDATORY", null));
            }
            if (string.IsNullOrEmpty(m_NewValue))
            {
                cse.AddElement(new PropertyException(this, "NewValue", "MANDATORY", null));
            }
            if (!m_CreatedAt.HasValue)
            {
                cse.AddElement(new PropertyException(this, "CreatedAt", "MANDATORY", null));
            }
            if (string.IsNullOrEmpty(m_CreatedBy))
            {
                cse.AddElement(new PropertyException(this, "CreatedBy", "MANDATORY", null));
            }
            return cse;
        }

        #endregion
    }
}
