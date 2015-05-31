using MultiDocument.Factories;
using MultiDocument.Interfaces;
using MultiDocument.Verifiers;
using System;
using System.Collections.Generic;

namespace MultiDocument.Common
{
    /// <summary>
    /// This is a base class that provides alias functionallity. The classes that want to have alias
    /// property should be inherited from this base class. It's a helper class that is used in the parser
    /// </summary>

    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public class BaseAliasAttribute : Attribute
    {
        #region Constructor

        public BaseAliasAttribute()
        {
        }

        public BaseAliasAttribute(string alias)
        {
            Alias = alias;
        }

        #endregion Constructor

        #region Properties

        public string Alias { get; set; }

        #endregion Properties
    }

    /// <summary>
    /// This attribute should be applied for members (properties and fields) those values can be saved and 
    /// restored during reading and writing operations. The readers and writers will try to process only members with
    /// those attribute.
    /// </summary>
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public class ProcessableAttribute : BaseAliasAttribute
    {
        #region Constructor

        public ProcessableAttribute()
            : this(null)
        {
        }

        public ProcessableAttribute(string alias)
            : base(alias)
        {
            this.SerializerFactory = new BasicDataSerializerFactory();
            this.Rank = -1;
        }

        public ProcessableAttribute(string alias, DataSerializerFactory serializerFactory)
            : this(alias)
        {
            this.SerializerFactory = serializerFactory;
        }

        public ProcessableAttribute(string alias, IDataVerifier dataVerifier)
            : this(alias)
        {
            this.DataVerifier = dataVerifier;
        }

        #endregion Constructor

        #region Properties

        public DataSerializerFactory SerializerFactory { get; set; }

        public IDataVerifier DataVerifier { get; set; }

        public int Rank { get; set; }

        #endregion Properties
    }
}
