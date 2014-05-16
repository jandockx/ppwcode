﻿using System;
using System.Collections.Concurrent;
using System.Diagnostics.Contracts;

using NHibernate;
using NHibernate.Type;

using PPWCode.Vernacular.nHibernate.I.Interfaces;
using PPWCode.Vernacular.Persistence.II;

namespace PPWCode.Vernacular.nHibernate.I.Utilities
{
    public class AuditInterceptor<T> : EmptyInterceptor
        where T : IEquatable<T>
    {
        private const string CreatedAtPropertyName = "CreatedAt";
        private const string CreatedByPropertyName = "CreatedBy";
        private const string LastModifiedAtPropertyName = "LastModifiedAt";
        private const string LastModifiedByPropertyName = "LastModifiedBy";

        private readonly IIdentityProvider m_IdentityProvider;
        private readonly ConcurrentDictionary<Property, int> m_IndexCache = new ConcurrentDictionary<Property, int>();
        private readonly ITimeProvider m_TimeProvider;

        public AuditInterceptor(IIdentityProvider identityProvider, ITimeProvider timeProvider)
        {
            if (identityProvider == null)
            {
                throw new ArgumentNullException("identityProvider");
            }
            if (timeProvider == null)
            {
                throw new ArgumentNullException("timeProvider");
            }
            Contract.EndContractBlock();

            m_IdentityProvider = identityProvider;
            m_TimeProvider = timeProvider;
        }

        private IIdentityProvider IdentityProvider
        {
            get { return m_IdentityProvider; }
        }

        private ITimeProvider TimeProvider
        {
            get { return m_TimeProvider; }
        }

        private ConcurrentDictionary<Property, int> IndexCache
        {
            get { return m_IndexCache; }
        }

        private void Set(Type entityType, string[] propertyNames, object[] state, string propertyName, object value)
        {
            int index = IndexCache.GetOrAdd(new Property(entityType, propertyName), k => Array.IndexOf(propertyNames, propertyName));
            if (index == -1)
            {
                return;
            }
            state[index] = value;
        }

        private bool SetAuditInfo(object entity, object[] currentState, string[] propertyNames, bool onSave)
        {
            IPersistentObject<T> persistentObject = entity as IPersistentObject<T>;
            if (persistentObject == null)
            {
                return false;
            }

            IInsertAuditable insertAuditable = entity as IInsertAuditable;
            IUpdateAuditable updateAuditable = entity as IUpdateAuditable;
            if (insertAuditable == null && updateAuditable == null)
            {
                return false;
            }

            DateTime time = TimeProvider.Now.ToUniversalTime();
            string identityName = IdentityProvider.IdentityName;
            if (identityName == null)
            {
                throw new InvalidOperationException("Unknown IdentityName");
            }
            Type entityType = entity.GetType();

            if (insertAuditable != null && (onSave || persistentObject.IsTransient))
            {
                IInsertAuditableProperties insertAuditableProperties = entity as IInsertAuditableProperties;
                string createdAtPropertyName =
                    insertAuditableProperties != null
                        ? insertAuditableProperties.CreatedAtPropertyName
                        : CreatedAtPropertyName;
                string createdByPropertyName =
                    insertAuditableProperties != null
                        ? insertAuditableProperties.CreatedByPropertyName
                        : CreatedByPropertyName;

                Set(entityType, propertyNames, currentState, createdAtPropertyName, time);
                Set(entityType, propertyNames, currentState, createdByPropertyName, identityName);

                insertAuditable.CreatedAt = time;
                insertAuditable.CreatedBy = identityName;
            }
            else if (updateAuditable != null)
            {
                IUpdateAuditableProperties updateAuditableProperties = entity as IUpdateAuditableProperties;
                string lastModifiedAtPropertyName =
                    updateAuditableProperties != null
                        ? updateAuditableProperties.LastModifiedAtPropertyName
                        : LastModifiedAtPropertyName;
                string lastModifiedByPropertyName =
                    updateAuditableProperties != null
                        ? updateAuditableProperties.LastModifiedByPropertyName
                        : LastModifiedByPropertyName;

                Set(entityType, propertyNames, currentState, lastModifiedAtPropertyName, time);
                Set(entityType, propertyNames, currentState, lastModifiedByPropertyName, identityName);

                updateAuditable.LastModifiedAt = time;
                updateAuditable.LastModifiedBy = identityName;
            }

            return true;
        }

        /// <summary>
        /// Called when an object is detected to be dirty, during a flush.
        /// </summary>
        /// <param name="currentState"/><param name="entity"/><param name="id"/><param name="previousState"/><param name="propertyNames"/><param name="types"/>
        /// <remarks>
        /// The interceptor may modify the detected <c>currentState</c>, which will be propagated to
        ///             both the database and the persistent object. Note that all flushes end in an actual
        ///             synchronization with the database, in which as the new <c>currentState</c> will be propagated
        ///             to the object, but not necessarily (immediately) to the database. It is strongly recommended
        ///             that the interceptor <b>not</b> modify the <c>previousState</c>.
        /// </remarks>
        /// <returns>
        /// <see langword="true"/> if the user modified the <c>currentState</c> in any way
        /// </returns>
        public override bool OnFlushDirty(object entity, object id, object[] currentState, object[] previousState, string[] propertyNames, IType[] types)
        {
            return SetAuditInfo(entity, currentState, propertyNames, false);
        }

        /// <summary>
        /// Called before an object is saved
        /// </summary>
        /// <param name="entity"/><param name="id"/><param name="propertyNames"/><param name="state"/><param name="types"/>
        /// <remarks>
        /// The interceptor may modify the <c>state</c>, which will be used for the SQL <c>INSERT</c>
        ///             and propagated to the persistent object
        /// </remarks>
        /// <returns>
        /// <see langword="true"/> if the user modified the <c>state</c> in any way
        /// </returns>
        public override bool OnSave(object entity, object id, object[] state, string[] propertyNames, IType[] types)
        {
            return SetAuditInfo(entity, state, propertyNames, true);
        }

        private struct Property : IEquatable<Property>
        {
            private readonly Type m_EntityType;
            private readonly string m_PropertyName;

            public Property(Type entityType, string propertyName)
            {
                m_EntityType = entityType;
                m_PropertyName = propertyName;
            }

            /// <summary>
            /// Indicates whether the current object is equal to another object of the same type.
            /// </summary>
            /// <returns>
            /// true if the current object is equal to the <paramref name="other"/> parameter; otherwise, false.
            /// </returns>
            /// <param name="other">An object to compare with this object.</param>
            public bool Equals(Property other)
            {
                return ReferenceEquals(m_EntityType, other.m_EntityType)
                       && string.Equals(m_PropertyName, other.m_PropertyName);
            }

            /// <summary>
            /// Indicates whether this instance and a specified object are equal.
            /// </summary>
            /// <returns>
            /// true if <paramref name="obj"/> and this instance are the same type and represent the same value; otherwise, false.
            /// </returns>
            /// <param name="obj">Another object to compare to. </param>
            public override bool Equals(object obj)
            {
                if (ReferenceEquals(null, obj))
                {
                    return false;
                }
                return obj is Property && Equals((Property)obj);
            }

            /// <summary>
            /// Returns the hash code for this instance.
            /// </summary>
            /// <returns>
            /// A 32-bit signed integer that is the hash code for this instance.
            /// </returns>
            public override int GetHashCode()
            {
                unchecked
                {
                    return (m_EntityType.GetHashCode() * 397) ^ m_PropertyName.GetHashCode();
                }
            }

            public static bool operator ==(Property left, Property right)
            {
                return left.Equals(right);
            }

            public static bool operator !=(Property left, Property right)
            {
                return !left.Equals(right);
            }
        }
    }
}