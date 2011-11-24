using System.Collections.Generic;
using System.IO;

namespace AI_.Studmix.ApplicationServices.DataTransferObjects
{
    public class ContentPackageDto
    {
        /// <summary>
        ///   Словарь пары ID свойства - название свойства.
        /// </summary>
        public IDictionary<int, string> Properties { get; set; }

        /// <summary>
        ///   Словарь пары ID свойства - состояние.
        /// </summary>
        public Dictionary<int, string> States { get; set; }

        public Dictionary<string, Stream> ContentFiles { get; set; }

        public Dictionary<string, Stream> PreviewContentFiles { get; set; }

        public string Caption { get; set; }

        public string Description { get; set; }

        public int Price { get; set; }

        public string OwnerUserName { get; set; }
    }
}