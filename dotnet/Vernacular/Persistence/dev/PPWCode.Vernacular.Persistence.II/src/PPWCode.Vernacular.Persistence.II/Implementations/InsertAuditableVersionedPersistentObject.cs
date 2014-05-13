﻿using System;
using System.Runtime.Serialization;

namespace PPWCode.Vernacular.Persistence.II
{
    [Serializable, DataContract(IsReference = true)]
    public abstract class InsertAuditableVersionedPersistentObject<T, TVersion>
        : VersionedPersistentObject<T, TVersion>,
          IInsertAuditable
        where T : IEquatable<T>
    {
        protected InsertAuditableVersionedPersistentObject(T id, TVersion persistenceVersion)
            : base(id, persistenceVersion)
        {
        }

        protected InsertAuditableVersionedPersistentObject(T id)
            : base(id)
        {
        }

        protected InsertAuditableVersionedPersistentObject()
        {
        }

        [DataMember, AuditLogPropertyIgnore]
        public virtual DateTime? CreatedAt { get; set; }

        [DataMember, AuditLogPropertyIgnore]
        public virtual string CreatedBy { get; set; }
    }
}