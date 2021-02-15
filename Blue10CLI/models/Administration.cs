using System;
using System.Xml.Serialization;

namespace Blue10CLI.models
{
    [XmlRoot("Administration")]
    public class Administration
    {
     
        [XmlElement]
        public string Id { get; set; }
        [XmlElement]
        public string AdministrationCode { get; set; }
        [XmlElement]
        public string LoginStatus { get; set; }
        [XmlElement]
        public string AdministrationVatNumber { get; set; }
        [XmlElement]
        public string AdministrationCurrencyCode { get; set; }

      
        public Administration(){}
    }
}