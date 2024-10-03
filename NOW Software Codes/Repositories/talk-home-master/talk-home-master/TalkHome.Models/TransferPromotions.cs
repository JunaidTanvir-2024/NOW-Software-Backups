using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

/*
<item>
<title>
Claro Honduras USD From 01 May 2018 00:00 To 02 May 2018 23:59 (GMT-06:00)
</title>
<description>
<![CDATA[
- Calls to all networks in Honduras, USA &amp; Canada<br />- Text messages to all networks.<br /><br />Promotional Balance Exceptions cannot be used for:<br />- The promotional balance does not apply for customers with the 10x1 promotion.<br />- Customer with the Universitario Plan<br /><br /><br />T&eacute;rminos y condiciones(Spanish version):<br /><br />- Llamadas a todas las redes en&nbsp;Honduras, USA y Canad&aacute;<br />- Mensajes a todas las redes<br />&nbsp;<br />Excepciones del balance promocional(para qu & eacute; no se puede usar):<br />- Saldo&nbsp;promocional no aplica a clientes que se encuentren en promoci&oacute;n de 10x1.<br />-&nbsp;Clientes con plan universitario
]]>
</description>
<dateFrom>Tue, 01 May 2018 06:00:00 +0000</dateFrom>
<dateTo>Thu, 03 May 2018 05:59:00 +0000</dateTo>
<pubDate>Mon, 30 Apr 2018 19:37:38 +0000</pubDate>
<operatorName>Claro Honduras USD</operatorName>
<operatorId>1584</operatorId>
<subOperatorId>1518,1584</subOperatorId>
<countryId>762</countryId>
<countryName>Honduras</countryName>
<title2>
<![CDATA[Bonus 6x]]>
</title2>
<denomination>
<![CDATA[USD 10 and up]]>
</denomination>
<denominationLocal>
<![CDATA[HNL 300 and up]]>
</denominationLocal>
</item>
*/

namespace TalkHome.Models
{
    public enum PromotionCategory
    {
        Current,
        Future
    }

    [XmlRoot("channel")]
    public class TransferPromotionItems
    {
        [XmlElement("item")]
        public List<TransferPromotionItem> TransferPromotions { get; set; }
    }

    public class TransferPromotionItem
    {
        private string dateFrom;
        private string dateTo;
        private PromotionCategory category;
        private string localDenomination;
        private string denomination;

        [XmlElement("description")]
        public string Description { get; set; }
        [XmlElement("title2")]
        public string Title { get; set; }
        [XmlElement("operatorName")]
        public string OperatorName { get; set; }
        [XmlElement("dateFrom")]
        public string DateFrom { get { return dateFrom; }
            set {
                DateTime fd = DateTime.Parse(value, null, System.Globalization.DateTimeStyles.RoundtripKind);
                dateFrom = fd.ToString("r");
                if (fd > DateTime.Now)
                {
                    category = PromotionCategory.Future;
                }
                else
                {
                    category = PromotionCategory.Current;
                }
            }
        }
        [XmlElement("dateTo")]
        public string DateTo { get { return dateTo; } set { dateTo = DateTime.Parse(value, null, System.Globalization.DateTimeStyles.RoundtripKind).ToString("r"); } }
        [XmlElement("pubDate")]
        public string PubDate { get; set; }
        [XmlElement("countryName")]
        public string CountryName { get; set; }
        [XmlElement("denomination")]
        public string Denomination { 
            set {
                denomination = value;
            }
            get
            {
                if (denomination == "![CDATA[ ]]")
                    return localDenomination;
                else
                    return denomination;

            }
        }
        [XmlElement("denominationLocal")]
        public string DenominationLocal {
            set {
                localDenomination = value;
            }
            get
            {
                if (String.IsNullOrEmpty(localDenomination))
                    return denomination;
                else
                    return localDenomination;
            }
        }
        public PromotionCategory Category { get { return category; } }

    }
}
