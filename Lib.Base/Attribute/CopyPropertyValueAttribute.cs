using System;

namespace Lib.Base
{
    /// <summary>
    /// UpdateSource
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class CopyPropertyValueAttribute : Attribute
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="isCopyPropertyValue"></param>
        public CopyPropertyValueAttribute(bool isCopyPropertyValue = true)
        {
            IsCopyPropertyValue = isCopyPropertyValue;
        }

        /// <summary>
        /// 
        /// </summary>
        public bool IsCopyPropertyValue
        {
            private set;
            get;
        }

    }
}
