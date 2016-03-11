using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using SeDaTec.SDatabase;
using SeDaTec.SDatabase.mTables;
using CO = SeDaTec.SDatabase.mTables.Configuration;
using System.ComponentModel;
using SeDaTec.sdUtility;
using DataBaseControl;

namespace Configuration2.UserControls
{
    public partial class Network : UserControl
    {
        private int p_preID_Configuration = -1;
        private BindingList<DG_NETWORK> DG_Network_Items = new BindingList<DG_NETWORK>();

        private List<IDataGridItem> ConvertToListIDataGrid()
        {
            List<IDataGridItem> l_Items = new List<IDataGridItem>();
            foreach (var item in DG_Network_Items)
            {
                l_Items.Add(item as IDataGridItem);
            }
            return l_Items;
        }
        private void btnAddChannel_Click(object sender, RoutedEventArgs e)
        {
            if (GlobalSettings.ID_Configuration == -1)
            { MessageBox.Show("Не выбрана конфигурация!", "Предупреждение", MessageBoxButton.OK, MessageBoxImage.Warning); return; }

            //Подключаемся к БД Oracle, SQL, SQLite
            string ret_STR = "";
            IDataBase_S l_DB;
            if (!SeDaTec.sdUtility.DataBaseUttility.ConnectDB_Full(out l_DB, true)) { return; }

            //Создаем элемент таблицы
            DG_NETWORK l_NEW_Network = new DG_NETWORK();

            //Создаем объект БД
            CO.c_Network l_newNetwork = new CO.c_Network();
            CO.m_Network l_newNetworkItem = new CO.m_Network()
            {
                NAME = l_NEW_Network.Name,
                IP = l_NEW_Network.IP,
                MASK = l_NEW_Network.MASK,
                AVAILABLE = true,
                L_CONFIG_ID = GlobalSettings.ID_Configuration
            };

            //Записываем новый элемент в БД
            l_newNetwork.Items.Add(l_newNetworkItem);
            bool t1 = l_DB.Write(l_newNetwork, out ret_STR);

            if (l_newNetworkItem.NETWORK_ID == -1)
            { l_DB.Connect_Close(out ret_STR); return; }

            //Связываем элемент таблицы с конфигураций и ID БД
            l_NEW_Network.ID = l_newNetworkItem.NETWORK_ID;
            l_NEW_Network.ID_Config = GlobalSettings.ID_Configuration;

            l_NEW_Network.ItemDB = l_newNetwork;

            l_DB.Connect_Close(out ret_STR);
            //Вывод нового элемента в таблицу
            DG_Network_Items.Add(l_NEW_Network);

            l_NEW_Network.onCheckItem += new CheckItem(l_NEW_Network_onCheckItem);

        }
        private void buttonSave_Click(object sender, RoutedEventArgs e)
        {
            ChangeIsEnabled(false);
            UtilityConfiguration.CheckAndSave(false, GlobalSettings.ID_Configuration, ConvertToListIDataGrid());
            ChangeIsEnabled(false);
        }
        private void buttonDelete_Click(object sender, RoutedEventArgs e)
        {
            if (dtGrid.SelectedItem == null)
            {
                MessageBox.Show("Выберите \"Сетевой адаптер\", который необходимо удалить.", "Удаление", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            IDataGridItem l_deleteItem = dtGrid.SelectedItem as IDataGridItem;
            if (UtilityConfiguration.Delete(l_deleteItem))
            {
                DG_Network_Items.Remove(l_deleteItem as DG_NETWORK);
            }
        }

        public Network()
        {
            InitializeComponent();
            dtGrid.ItemsSource = DG_Network_Items;
        }
        public void ViewNetwork()
        {
            ///////////////////////////////////////////При смени конфигурации запускаем проверку/////////////////
            if (p_preID_Configuration != -1)
            {
                UtilityConfiguration.CheckAndSave(true, p_preID_Configuration, ConvertToListIDataGrid());
            }
            p_preID_Configuration = GlobalSettings.ID_Configuration;
            /////////////////////////////////////////////////////////////////////////////////////////////////////

            //Подключение к БД
            string ret_STR = "";
            IDataBase_S l_DB;
            if (!SeDaTec.sdUtility.DataBaseUttility.ConnectDB_Full(out l_DB, true)) { return; }

            //Выгрузка из любой БД Oracle, SQL, SQLite
            CO.c_Network l_NETWORK_DB = new CO.c_Network();
            DataBaseUttility.Read_Available_AND_ID_Config(l_DB, l_NETWORK_DB, GlobalSettings.ID_Configuration);
            l_DB.Connect_Close(out ret_STR);

            //Создаем привязку данных к таблице
            DG_Network_Items = new BindingList<DG_NETWORK>();
            foreach (var network in l_NETWORK_DB.Items)
            {
                DG_NETWORK l_newNetworkItem = new DG_NETWORK()
                {
                    Name = network.NAME,
                    IP = network.IP,
                    MASK = network.MASK,
                    ID = network.NETWORK_ID,
                    ID_Config = network.L_CONFIG_ID,
                };
                CO.c_Network l_newItemDB = new CO.c_Network();
                l_newItemDB.Items.Add(network);
                l_newNetworkItem.ItemDB = l_newItemDB;

                DG_Network_Items.Add(l_newNetworkItem);
                l_newNetworkItem.onCheckItem += new CheckItem(l_NEW_Network_onCheckItem);
            }
            dtGrid.ItemsSource = DG_Network_Items;
            ChangeIsEnabled(false);
        }
        //Иконка "Сохранить" изм. состояния
        public void ChangeIsEnabled(bool isEnabledBtnSave)
        {

            buttonSave.IsEnabled = isEnabledBtnSave;

            if (isEnabledBtnSave)
            {
                Uri uriii = new Uri("pack://application:,,,/Icon/filesave_5443.png");
                BitmapImage bitmap = new BitmapImage(uriii);
                Image IMG = new Image();
                imgSave.Source = bitmap;
            }
            else
            {
                Uri uriii = new Uri("pack://application:,,,/Icon/filesave_5443_2.png");
                BitmapImage bitmap = new BitmapImage(uriii);
                Image IMG = new Image();
                imgSave.Source = bitmap;
            }

        }
        void l_NEW_Network_onCheckItem(bool f_isCheckUpdate)
        {
            ChangeIsEnabled(f_isCheckUpdate);
        }
    }

    public class DG_NETWORK : DataGridItem
    {
        ///////////////////////////////////Реализация в каждом DataGridItem//////////////////////////////////////
        public override string NameItems { get { return "\"Сетевой адаптер\""; } }

        public override bool Check()
        {
            if (ItemDB == null) { return true; }

            CO.c_Network l_NETWORK = ItemDB as CO.c_Network;
            if (Name != l_NETWORK.Items.First().NAME) { return false; }
            if (IP != l_NETWORK.Items.First().IP) { return false; }
            if (MASK != l_NETWORK.Items.First().MASK) { return false; }

            return true;
        }
        public override void Update()
        {
            CO.c_Network l_NETWORK = ItemDB as CO.c_Network;
            l_NETWORK.Items.First().NAME = Name;
            l_NETWORK.Items.First().IP = IP;
            l_NETWORK.Items.First().MASK = MASK;

            l_NETWORK.EnabledUpdate(true);
        }
        ///////////////////////////////////Реализация в каждом IDataGridItem//////////////////////////////////////

        public DG_NETWORK()
        {
            Tele = new CO.c_Tele();
            TeleValue = new CO.c_Tele_Value();
            ID_Config = -1;
        }
        public int ID { get; set; }
        public int ID_Config { get; set; }

        public string _Name = "на МВКС";
        public string _IP = "192.168.14.1";
        public string _MASK = "255.255.255.0";

        public string Name
        {
            get { return _Name; }
            set
            {
                _Name = value;
                OnPropertyChanged("Name");
                CheckItemUpdate();
                
            }
        }
        public string IP
        {
            get { return _IP; }
            set
            {
                _IP = value;
                OnPropertyChanged("IP");
                CheckItemUpdate();
            }
        }
        public string MASK
        {
            get { return _MASK; }
            set
            {
                _MASK = value;
                OnPropertyChanged("MASK");
                CheckItemUpdate();
            }
        }
    }
}
