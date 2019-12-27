using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EnsureRisk.Classess
{
    public class HeaderExcelContent
    {
        public string MyContent { get; set; }

        public int Column{ get; set; }

        public int IdClasification { get; set; }

        public HeaderExcelContent(string MyContent, int Column)
        {
            this.MyContent = MyContent;
            this.Column = Column;
        }
        public HeaderExcelContent()
        {

        }
    }
}
