using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace SeDaTec.SDatabase.mTables.Configuration
{
    public class mTables_Configuration : ModelT
    {
        public mTables_Configuration()
        {
            Description = "Таблички для Конфигурации";
            Version_T = "2.0.5";
            Name = "Конфигуратор КЛ";
            ID = 100;

            Network = new c_Network();
            Network.NameViewUser = "№015 [Сеть]";
        }
        public override string ToString()
        {
            return Name;
        }
        public c_Network Network { get; set; }
     
    }
	
    //////////////////////////////////////////////Model Network///////////////////////////////////////////////////////
    public enum e_m_Network { NETWORK_ID, NAME, IP, MASK, AVAILABLE, L_CONFIG_ID, ID_Server }
    [Serializable]
    public class m_Network
    {
        [XmlAttribute("NETWORK_ID")]
        public int NETWORK_ID = -1;
        [XmlAttribute("NAME")]
        public string NAME = "";
        [XmlAttribute("IP")]
        public string IP = "";
        [XmlAttribute("MASK")]
        public string MASK = "";
        [XmlAttribute("AVAILABLE")]
        public bool AVAILABLE = true;
        [XmlAttribute("L_CONFIG_ID")]
        public int L_CONFIG_ID = -1;
        [XmlAttribute("ID_Server")]
        public int ID_Server = 0;
    }
    public class c_Network : ModelTables, IAdvancedModelTables
    {
        [XmlElement("Item")]
        public List<m_Network> Items = new List<m_Network>();
        public c_Network()
        {

            NameTable = "mc_Network";

            ListParam.ListParametersCol.Add(new ParamColumn("NETWORK_ID", true, true, "s_mc_Network", 1, e_TypeColumns.Int32, true));
            ParamName = new ParamColumn("NAME", e_TypeColumns.String_100, true); ListParam.ListParametersCol.Add(ParamName);
            ListParam.ListParametersCol.Add(new ParamColumn("IP", e_TypeColumns.String_100, true));
            ListParam.ListParametersCol.Add(new ParamColumn("MASK", e_TypeColumns.String_100, true));
            ParamAvailable = new ParamColumn("AVAILABLE", e_TypeColumns.Bool, true);
            ListParam.ListParametersCol.Add(ParamAvailable);
            ParamIDConfig = new ParamColumn("L_CONFIG_ID", e_TypeColumns.Int32, true);
            ListParam.ListParametersCol.Add(ParamIDConfig);

            ParamColumn l_Server = new ParamColumn("ID_Server", e_TypeColumns.Int32, false); l_Server.isRead = false; l_Server.isWrite = false;
            ListParam.ListParametersCol.Add(l_Server);
        }

        public virtual void EnabledUpdate(bool f_isEnabled)
        {
            GetParam(e_m_Network.IP.ToString()).isUpdate = f_isEnabled;
            GetParam(e_m_Network.NAME.ToString()).isUpdate = f_isEnabled;
            GetParam(e_m_Network.MASK.ToString()).isUpdate = f_isEnabled;
        }
        public override object GetNewItem()
        {
            m_Network l_newItem = new m_Network();
            Items.Add(l_newItem);
            return l_newItem;
        }
        [XmlIgnore()]
        public virtual ParamColumn ParamAvailable { get; set; }
        [XmlIgnore()]
        public virtual ParamColumn ParamName { get; set; }
        [XmlIgnore()]
        public virtual ParamColumn ParamIDConfig { get; set; }
        public virtual int GetIDFirstElement()
        {
            return Items.First().NETWORK_ID;
        }
        public virtual void ChangeAvailable(bool ChangeAvb)
        {
            foreach (var item in Items)
            {
                item.AVAILABLE = ChangeAvb;
            }
        }
        [XmlIgnore()]
        public virtual ParamColumn ParamSort { get; set; }
        [XmlIgnore()]
        public virtual int SortFirstItem { get { return int.MinValue; } set { } }
        [XmlIgnore()]
        public virtual string NameFirstElement { get { return Items.First().NAME; } set { Items.First().NAME = value; } }
        public virtual LinkItem GetNextItem(int ID)
        {
            LinkItem l_newLink = new LinkItem();

            m_Network l_FindItem = Items.Find(x => x.NETWORK_ID == ID);
            if (l_FindItem == null) { return null; }
            l_newLink.ID_Link = l_FindItem.L_CONFIG_ID;
            l_newLink.NameTable = new c_Configuration().NameTable;
            l_newLink.Name = l_FindItem.NAME;

            return l_newLink;
        }
        public virtual int GetMaxIDIndex()
        {
            if (Items.Count == 0) { return 1; }
            return Items.Max(x => x.NETWORK_ID);
        }
    }

}
