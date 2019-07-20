using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Microsoft.Office.Interop.Excel;
using Microsoft.Office.Core;
using System.Threading;
using DevExpress.XtraEditors;
using System.IO;

unsafe public struct VCI_CAN_OBJ  //使用不安全代码
{
    public uint ID;
    public uint TimeStamp;
    public byte TimeFlag;
    public byte SendType;
    public byte RemoteFlag;//是否是远程帧
    public byte ExternFlag;//是否是扩展帧
    public byte DataLen;
    public fixed byte Data[8];
    public fixed byte Reserved[3];
}
//定义初始化CAN的数据类型
public struct VCI_INIT_CONFIG
{
    public UInt32 AccCode;
    public UInt32 AccMask;
    public UInt32 Reserved;
    public byte Filter;
    public byte Timing0;
    public byte Timing1;
    public byte Mode;
}
namespace 主控beta1._1
{
    public partial class Form1 : Form
    {
        #region 导入DLL
        [DllImport("controlcan.dll")]
        static extern UInt32 VCI_OpenDevice(UInt32 DeviceType, UInt32 DeviceInd, UInt32 Reserved);
        [DllImport("controlcan.dll")]
        static extern UInt32 VCI_CloseDevice(UInt32 DeviceType, UInt32 DeviceInd);
        [DllImport("controlcan.dll")]
        static extern UInt32 VCI_InitCAN(UInt32 DeviceType, UInt32 DeviceInd, UInt32 CANInd, ref VCI_INIT_CONFIG pInitConfig);

        [DllImport("controlcan.dll")]
        static extern UInt32 VCI_GetReference(UInt32 DeviceType, UInt32 DeviceInd, UInt32 CANInd, UInt32 RefType, ref byte pData);
        [DllImport("controlcan.dll")]
        static extern UInt32 VCI_SetReference(UInt32 DeviceType, UInt32 DeviceInd, UInt32 CANInd, UInt32 RefType, ref byte pData);

        [DllImport("controlcan.dll")]
        static extern UInt32 VCI_GetReceiveNum(UInt32 DeviceType, UInt32 DeviceInd, UInt32 CANInd);
        [DllImport("controlcan.dll")]
        static extern UInt32 VCI_ClearBuffer(UInt32 DeviceType, UInt32 DeviceInd, UInt32 CANInd);

        [DllImport("controlcan.dll")]
        static extern UInt32 VCI_StartCAN(UInt32 DeviceType, UInt32 DeviceInd, UInt32 CANInd);
        [DllImport("controlcan.dll")]
        static extern UInt32 VCI_ResetCAN(UInt32 DeviceType, UInt32 DeviceInd, UInt32 CANInd);

        [DllImport("controlcan.dll")]
        static extern UInt32 VCI_Transmit(UInt32 DeviceType, UInt32 DeviceInd, UInt32 CANInd, ref VCI_CAN_OBJ pSend, UInt32 Len);

        [DllImport("controlcan.dll", CharSet = CharSet.Ansi)]
        static extern UInt32 VCI_Receive(UInt32 DeviceType, UInt32 DeviceInd, UInt32 CANInd, IntPtr pReceive, UInt32 Len, Int32 WaitTime);


        #endregion
        static UInt32 m_devtype = 3;//USBCAN1
        public static UInt32 m_bOpen = 0;
        static UInt32 m_devind = 0;
        static UInt32 m_canind = 0;
        UInt32[] m_arrdevtype = new UInt32[5];
        public static bool intimer = false;
        public static bool 运行 = false;
        public static bool 报文 = false;
        private static bool savedata = false;
        public static UInt32[] v1 = new UInt32[16];
        public static Int32[] t1 = new Int32[8];
        public static string[] temp = new string[120];
        public static UInt32[] tempv = new UInt32[40];
        public static UInt32 AveVoltage;
        public static UInt32 MaxVoltage;
        public static UInt32 MinVoltage;
        public static UInt32 MaxVoltageNO;
        public static UInt32 MinVoltageNO;
        public static Int32 AveTemperture;
        public static Int32 MaxTemperture;
        public static Int32 MinTemperture;
        public static UInt32 MaxTempertureNO;
        public static UInt32 MinTempertureNO;
        public static UInt32 均衡状态1;
        public static UInt32 均衡状态2;
        static UInt32 batv;
        static Int32 batvindex;
        static Int32 batt;
        static Int32 battindex;
        static Int32 bata;
        static Int32 bataindex;
        System.Threading.Timer timer1;
        private delegate void changepic(Image imag);
        private delegate void changelist();
        static private List<CheckEdit> listBox;
        public class Pcs变量
        {
            public static Double 额定电压;
            public static UInt32 额定容量;
            public static UInt32 从控节点数量;
            public static string 电池类型;
            public static UInt32 电流传感器类型;
            public static UInt32 温度传感器类型;
            public static Double 单体最低电压;
            public static UInt32 单体最低电压所在Pack号;
            public static UInt32 单体最低电压所在Pack内编号;
            public static Double 单体最高电压;
            public static UInt32 单体最高电压所在Pack号;
            public static UInt32 单体最高电压所在Pack内编号;
            public static Int64 最低温度;
            public static UInt32 最低温度所在Pack号;
            public static UInt32 最低温度所在Pack内编号;
            public static Int64 最高温度;
            public static Int64 MOS温度;
            public static UInt32 最高温度所在Pack号;
            public static UInt32 最高温度所在Pack内编号;
            public static UInt32 容量;
            public static UInt32 电压个数;
            public static UInt32 温度个数;
            public static Double 总电压;
            public static Double 实时电流;
            public static UInt32 SOC;
            public static UInt32 SOH;
            public static UInt32 BMS充电次数;
            public static UInt32 最大允许充电电压;
            public static UInt32 最小允许放电电压;
            public static UInt32 最大允许充电电流;
            public static UInt32 最大允许放电电流;
            public static string BMS最高告警故障状态;
            public static string Ready;
            public static string BMS工作状态;
            public static string BMU1链接故障;
            public static string BMU2链接故障;
            public static string BMU3链接故障;
            public static string BMU4链接故障;
            public static string BMU5链接故障;
            public static string BMU6链接故障;
            public static string BMU7链接故障;
            public static string BMU8链接故障;
            public static string BMU9链接故障;
            public static string BMU10链接故障;
            public static string PCS链接故障;
            public static string 轻微漏电;
            public static string 轻微单体过高;
            public static string 轻微单体过低;
            public static string 轻微总压过高;
            public static string 轻微总压过低;
            public static string 轻微MOS温度过高;
            public static string 轻微充电电流过高;
            public static string 轻微放电电流过高;
            public static string 轻微充电温度过高;
            public static string 轻微充电温度过低;
            public static string 轻微放电温度过高;
            public static string 轻微放电温度过低;
            public static string 轻微单体压差过大;
            public static string 轻微温差过大;
            public static string 轻微SOC过低;
            public static string 轻微SOC过高;
            public static string 一般漏电;
            public static string 一般单体过高;
            public static string 一般单体过低;
            public static string 一般总压过高;
            public static string 一般总压过低;
            public static string 一般MOS温度过高;
            public static string 一般充电电流过高;
            public static string 一般放电电流过高;
            public static string 一般充电温度过高;
            public static string 一般充电温度过低;
            public static string 一般放电温度过高;
            public static string 一般放电温度过低;
            public static string 一般单体压差过大;
            public static string 一般温差过大;
            public static string 一般SOC过低;
            public static string 一般SOC过高;
            public static string 严重漏电;
            public static string 严重单体过高;
            public static string 严重单体过低;
            public static string 严重总压过高;
            public static string 严重总压过低;
            public static string 严重MOS温度过高;
            public static string 严重充电电流过高;
            public static string 严重放电电流过高;
            public static string 严重充电温度过高;
            public static string 严重充电温度过低;
            public static string 严重放电温度过高;
            public static string 严重放电温度过低;
            public static string 严重单体压差过大;
            public static string 严重温差过大;
            public static string 严重SOC过低;
            public static string 严重SOC过高;

            public static string 绝缘模块连接故障;
            public static string 烟雾传感器连接故障;
            public static string 电流传感器故障;
            public static string 温度传感器故障;
            public static string BMU电压故障;
            public static string 烟雾传感器故障;
            public static string 加热故障;
            public static string 单体电压过低告警;
            public static string 单体电压过高告警;
            public static string 单体压差大告警;
            public static string 温度过低告警;
            public static string 温度过高告警;
            public static string 温差大告警;
            public static string 放电SOC过低告警;
            public static string 电流过高告警;
            public static string 绝缘漏电告警;
            public static string 烟雾报警;
            public static string 高压压差大告警;
            public static string 充放电禁止状态;
            public static string 高压正接触器状态;
            public static string 高压负接触器状态;
            public static string 预充接触器状态;
            public static string 加热正接触器状态;
            public static string 加热负接触器状态;
            public static string 风扇接触器状态;
            public static string 放电温度过低告警;
            public static string 放电温度过高报警;
            public static string 充电接触器状态;
            public static string 放电接触器状态;
            public static UInt32 BMS累计充放电次数;
            public static UInt32 电池序列号;
        }

        private string RecStartTime = DateTime.Now.ToString("MM月dd日HH时mm分ss秒");

        private string RecRealTime;
        private string fileName;
        static String canstr = "";
        private Microsoft.Office.Interop.Excel.Application ExcelApp;
        private Workbook ExcelDoc;
        private dynamic xlSheet;
        private UInt32 Row;
        //private bool ReleaseFlag;
        static callback callback1 = new callback();
        private delegate void changecanlist();
        public Form1()
        {
            InitializeComponent(); 连接pb1.Click += new EventHandler(windowsUIButtonPanel1_Click); initialdisplay(); intialtimer(); Init();
            CAN索引.SelectedIndex = 0; CAN通道.SelectedIndex = 0;
            单体电压值.Enabled = false; 温度值.Enabled = false; 电流值.Enabled = false; 保存.Enabled = false;
            电池号.Enabled = false; 温度号.Enabled = false; 电流.Enabled = false;
            

        }
        public void Init() //初始化集合
        {
            listBox = new List<CheckEdit>(); //添加对象
            listBox.Add(开始模拟);
            SOC1.DataBindings.Add("Text", callback1, "ID1"); SOH1.DataBindings.Add("Text", callback1, "ID2"); 容量1.DataBindings.Add("Text", callback1, "ID3");
            从控电池串数1.DataBindings.Add("Text", callback1, "ID4"); 低压报警1_1.DataBindings.Add("Text", callback1, "ID5"); 低压报警2_1.DataBindings.Add("Text", callback1, "ID6");
            低压报警3_1.DataBindings.Add("Text", callback1, "ID7"); 高压报警1_1.DataBindings.Add("Text", callback1, "ID8"); 高压报警2_1.DataBindings.Add("Text", callback1, "ID9");
            高压报警3_1.DataBindings.Add("Text", callback1, "ID10"); 从控温度传感器数1.DataBindings.Add("Text", callback1, "ID11"); 单体满电电压1.DataBindings.Add("Text", callback1, "ID12");
            电池总个数1.DataBindings.Add("Text", callback1, "ID13"); 放电低温1_1.DataBindings.Add("Text", callback1, "ID14"); 放电低温2_1.DataBindings.Add("Text", callback1, "ID15");
            放电低温3_1.DataBindings.Add("Text", callback1, "ID16"); 最高温度报警1_1.DataBindings.Add("Text", callback1, "ID17"); 最高温度报警2_1.DataBindings.Add("Text", callback1, "ID18");
            最高温度报警3_1.DataBindings.Add("Text", callback1, "ID19"); 最大允许充电电流1_1.DataBindings.Add("Text", callback1, "ID21"); 最大允许充电电流2_1.DataBindings.Add("Text", callback1, "ID22");
            最大允许充电电流3_1.DataBindings.Add("Text", callback1, "ID23"); 最大允许放电电流1_1.DataBindings.Add("Text", callback1, "ID24"); 最大允许放电电流2_1.DataBindings.Add("Text", callback1, "ID25");
            最大允许放电电流3_1.DataBindings.Add("Text", callback1, "ID26"); 电压不均衡1_1.DataBindings.Add("Text", callback1, "ID27"); 电压不均衡2_1.DataBindings.Add("Text", callback1, "ID28");
            电压不均衡3_1.DataBindings.Add("Text", callback1, "ID29"); 温度不均衡1_1.DataBindings.Add("Text", callback1, "ID30"); 温度不均衡2_1.DataBindings.Add("Text", callback1, "ID31");
            温度不均衡3_1.DataBindings.Add("Text", callback1, "ID32"); SOC过低1_1.DataBindings.Add("Text", callback1, "ID33"); SOC过低2_1.DataBindings.Add("Text", callback1, "ID34");
            SOC过低3_1.DataBindings.Add("Text", callback1, "ID35"); MOS温度过高1_1.DataBindings.Add("Text", callback1, "ID36"); MOS温度过高2_1.DataBindings.Add("Text", callback1, "ID37");
            MOS温度过高3_1.DataBindings.Add("Text", callback1, "ID38"); 充电低温1_1.DataBindings.Add("Text", callback1, "ID39"); 充电低温2_1.DataBindings.Add("Text", callback1, "ID40");
            充电低温3_1.DataBindings.Add("Text", callback1, "ID41"); 充电高温1_1.DataBindings.Add("Text", callback1, "ID42"); 充电高温2_1.DataBindings.Add("Text", callback1, "ID43");
            充电高温3_1.DataBindings.Add("Text", callback1, "ID44"); 放电高温1_1.DataBindings.Add("Text", callback1, "ID45"); 放电高温2_1.DataBindings.Add("Text", callback1, "ID46");
            放电高温3_1.DataBindings.Add("Text", callback1, "ID47"); 轻微漏电时间1_1.DataBindings.Add("Text", callback1, "ID48"); 轻微漏电时间2_1.DataBindings.Add("Text", callback1, "ID49");
            轻微漏电时间3_1.DataBindings.Add("Text", callback1, "ID50"); 单体过高时间1_1.DataBindings.Add("Text", callback1, "ID51"); 单体过高时间2_1.DataBindings.Add("Text", callback1, "ID52");
            单体过高时间3_1.DataBindings.Add("Text", callback1, "ID53"); 单体过低时间1_1.DataBindings.Add("Text", callback1, "ID54"); 单体过低时间2_1.DataBindings.Add("Text", callback1, "ID55");
            单体过低时间3_1.DataBindings.Add("Text", callback1, "ID56"); 总压过高时间1_1.DataBindings.Add("Text", callback1, "ID57"); 总压过高时间2_1.DataBindings.Add("Text", callback1, "ID58");
            总压过高时间3_1.DataBindings.Add("Text", callback1, "ID59"); 总压过低时间1_1.DataBindings.Add("Text", callback1, "ID60"); 总压过低时间2_1.DataBindings.Add("Text", callback1, "ID61");
            总压过低时间3_1.DataBindings.Add("Text", callback1, "ID62"); MOS温度过高时间1_1.DataBindings.Add("Text", callback1, "ID63"); MOS温度过高时间2_1.DataBindings.Add("Text", callback1, "ID64");
            MOS温度过高时间3_1.DataBindings.Add("Text", callback1, "ID65"); 充电电流过高时间1_1.DataBindings.Add("Text", callback1, "ID66"); 充电电流过高时间2_1.DataBindings.Add("Text", callback1, "ID67");
            充电电流过高时间3_1.DataBindings.Add("Text", callback1, "ID68"); 放电电流过高时间1_1.DataBindings.Add("Text", callback1, "ID69"); 放电电流过高时间2_1.DataBindings.Add("Text", callback1, "ID70");
            放电电流过高时间3_1.DataBindings.Add("Text", callback1, "ID71"); 充电温度过高时间1_1.DataBindings.Add("Text", callback1, "ID72"); 充电温度过高时间2_1.DataBindings.Add("Text", callback1, "ID73");
            充电温度过高时间3_1.DataBindings.Add("Text", callback1, "ID74"); 充电温度过低时间1_1.DataBindings.Add("Text", callback1, "ID75"); 充电温度过低时间2_1.DataBindings.Add("Text", callback1, "ID76");
            充电温度过低时间3_1.DataBindings.Add("Text", callback1, "ID77"); 放电温度过高时间1_1.DataBindings.Add("Text", callback1, "ID78"); 放电温度过高时间2_1.DataBindings.Add("Text", callback1, "ID79");
            放电温度过高时间3_1.DataBindings.Add("Text", callback1, "ID80"); 放电温度过低时间1_1.DataBindings.Add("Text", callback1, "ID81"); 放电温度过低时间2_1.DataBindings.Add("Text", callback1, "ID82");
            放电温度过低时间3_1.DataBindings.Add("Text", callback1, "ID83"); 单体压差过大时间1_1.DataBindings.Add("Text", callback1, "ID84"); 单体压差过大时间2_1.DataBindings.Add("Text", callback1, "ID85");
            单体压差过大时间3_1.DataBindings.Add("Text", callback1, "ID86"); 温差过大时间1_1.DataBindings.Add("Text", callback1, "ID87"); 温差过大时间2_1.DataBindings.Add("Text", callback1, "ID88");
            温差过大时间3_1.DataBindings.Add("Text", callback1, "ID89"); SOC过低时间1_1.DataBindings.Add("Text", callback1, "ID90"); SOC过低时间2_1.DataBindings.Add("Text", callback1, "ID91");
            SOC过低时间3_1.DataBindings.Add("Text", callback1, "ID92");

            SOC.Text = "60"; SOH.Text = "100"; 容量.Text = "40";
            从控电池串数.Text = "16"; 低压报警1.Text = "3.1"; 低压报警2.Text = "3.0";
            低压报警3.Text = "2.9"; 高压报警1.Text = "4.12"; 高压报警2.Text = "4.14";
            高压报警3.Text = "4.15"; 从控温度传感器数.Text = "8"; 单体满电电压.Text = "4.15";
            放电低温1.Text = "0"; 放电低温2.Text = "-10"; 放电低温3.Text = "-15";
            最高温度报警1.Text = "50"; 最高温度报警2.Text = "55"; 最高温度报警3.Text = "60";
            电池总个数.Text = "14"; 放电低温1.Text = "-10"; 放电低温2.Text = "-15"; 放电低温3.Text = "-20"; 最高温度报警1.Text = "50";
            最高温度报警2.Text = "55"; 最高温度报警3.Text = "60"; 最大允许充电电流1.Text = "-15"; 最大允许充电电流2.Text = "-20";
            最大允许充电电流3.Text = "-25"; 最大允许放电电流1.Text = "15"; 最大允许放电电流2.Text = "20"; 最大允许放电电流3.Text = "25";
            电压不均衡1.Text = "300"; 电压不均衡2.Text = "400"; 电压不均衡3.Text = "500"; 温度不均衡1.Text = "4"; 温度不均衡2.Text = "5";
            温度不均衡3.Text = "6"; SOC过低1.Text = "20"; SOC过低2.Text = "15"; SOC过低3.Text = "10"; MOS温度过高1.Text = "60";
            MOS温度过高2.Text = "70"; MOS温度过高3.Text = "80"; 充电低温1.Text = "-10"; 充电低温2.Text = "-15"; 充电低温3.Text = "-20";
            充电高温1.Text = "45"; 充电高温2.Text = "50"; 充电高温3.Text = "60"; 放电高温1.Text = "45"; 放电高温2.Text = "50"; 放电高温3.Text = "55";
            开始模拟.Enabled = false;
        }
        private void initialdisplay()
        {
            dataGridView1.RowHeadersVisible = false;
            dataGridView2.RowHeadersVisible = false;
            dataGridView3.RowHeadersVisible = false;
            
            dataGridView5.RowHeadersVisible = false;
            dataGridView6.RowHeadersVisible = false;
            int index1 = 17; int index2 = 16; int index3 = 15;  int index5 = 17; int index6 = 11;
            dataGridView1.Rows.Add(index1); dataGridView2.Rows.Add(index2); dataGridView3.Rows.Add(index3);  dataGridView5.Rows.Add(index5); dataGridView6.Rows.Add(index6);
            dataGridView1.Rows[0].Cells[0].Value = "额定电压";
            dataGridView1.Rows[1].Cells[0].Value = "额定容量";
            //dataGridView1.Rows[2].Cells[0].Value = "从空节点数量";
            dataGridView1.Rows[2].Cells[0].Value = "电池类型";
            dataGridView1.Rows[3].Cells[0].Value = "电流传感器类型";
            dataGridView1.Rows[4].Cells[0].Value = "温度传感器类型";
            dataGridView1.Rows[5].Cells[0].Value = "单体最低电压";
            //dataGridView1.Rows[7].Cells[0].Value = "单体最低电压所在Pack号";
            dataGridView1.Rows[6].Cells[0].Value = "单体最低电压所在Pack内编号";
            dataGridView1.Rows[7].Cells[0].Value = "单体最高电压";
            //dataGridView1.Rows[10].Cells[0].Value = "单体最高电压所在Pack号";
            dataGridView1.Rows[8].Cells[0].Value = "单体最高电压所在Pack内编号";
            dataGridView1.Rows[9].Cells[0].Value = "平均电压";
            dataGridView1.Rows[10].Cells[0].Value = "最低温度";
            //dataGridView1.Rows[13].Cells[0].Value = "最低温度所在Pack号";
            dataGridView1.Rows[11].Cells[0].Value = "最低温度所在Pack内编号";
            dataGridView1.Rows[12].Cells[0].Value = "最高温度";
            //dataGridView1.Rows[16].Cells[0].Value = "最高温度所在Pack号";
            dataGridView1.Rows[13].Cells[0].Value = "最高温度所在Pack内编号";
            dataGridView1.Rows[14].Cells[0].Value = "平均温度";
            dataGridView1.Rows[15].Cells[0].Value = "电压个数";
            dataGridView1.Rows[16].Cells[0].Value = "温度个数";
            dataGridView1.Rows[17].Cells[0].Value = "MOS温度";
            //dataGridView2.Rows[0].Cells[0].Value = "实际容量";
            dataGridView2.Rows[0].Cells[0].Value = "总电压";
            dataGridView2.Rows[1].Cells[0].Value = "实时电流";
            dataGridView2.Rows[2].Cells[0].Value = "SOC";
            dataGridView2.Rows[3].Cells[0].Value = "SOH";
            dataGridView2.Rows[4].Cells[0].Value = "BMS充电次数";
            dataGridView2.Rows[5].Cells[0].Value = "最大允许充电电压";
            dataGridView2.Rows[6].Cells[0].Value = "最小允许放电电压";
            dataGridView2.Rows[7].Cells[0].Value = "最大允许充电电流";
            dataGridView2.Rows[8].Cells[0].Value = "最大允许放电电流";
            dataGridView2.Rows[9].Cells[0].Value = "BMS最高告警（故障）状态";
            dataGridView2.Rows[10].Cells[0].Value = "BMU电压故障";
            dataGridView2.Rows[11].Cells[0].Value = "BMS工作状态";
            dataGridView2.Rows[12].Cells[0].Value = "温度传感器故障";
            //dataGridView2.Rows[13].Cells[0].Value = "烟雾传感器连接故障";
            dataGridView2.Rows[13].Cells[0].Value = "电流传感器故障";
            dataGridView2.Rows[14].Cells[0].Value = "绝缘模块连接故障";
            dataGridView2.Rows[15].Cells[0].Value = "放电接触器状态";
            dataGridView2.Rows[16].Cells[0].Value = "充电接触器状态";
            //dataGridView2.Rows[17].Cells[0].Value = "烟雾传感器故障";
            //dataGridView2.Rows[18].Cells[0].Value = "加热故障";
            dataGridView3.Rows[0].Cells[0].Value = "漏电";
            dataGridView3.Rows[1].Cells[0].Value = "单体过高";
            dataGridView3.Rows[2].Cells[0].Value = "单体过低";
            dataGridView3.Rows[3].Cells[0].Value = "总压过高";
            dataGridView3.Rows[4].Cells[0].Value = "总压过低";
            dataGridView3.Rows[5].Cells[0].Value = "MOS温度过高";
            dataGridView3.Rows[6].Cells[0].Value = "充电电流过高";
            dataGridView3.Rows[7].Cells[0].Value = "放电电流过高";
            dataGridView3.Rows[8].Cells[0].Value = "充电温度过高";
            dataGridView3.Rows[9].Cells[0].Value = "充电温度过低";
            dataGridView3.Rows[10].Cells[0].Value = "放电温度过高";
            dataGridView3.Rows[11].Cells[0].Value = "放电温度过低";
            dataGridView3.Rows[12].Cells[0].Value = "单体压差过大";
            dataGridView3.Rows[13].Cells[0].Value = "温差过大";
            dataGridView3.Rows[14].Cells[0].Value = "SOC过低";
            dataGridView3.Rows[15].Cells[0].Value = "SOC过高";

            dataGridView5.Rows[0].Cells[0].Value = "单体1电压";
            dataGridView5.Rows[1].Cells[0].Value = "单体2电压";
            dataGridView5.Rows[2].Cells[0].Value = "单体3电压";
            dataGridView5.Rows[3].Cells[0].Value = "单体4电压";
            dataGridView5.Rows[4].Cells[0].Value = "单体5电压";
            dataGridView5.Rows[5].Cells[0].Value = "单体6电压";
            dataGridView5.Rows[6].Cells[0].Value = "单体7电压";
            //dataGridView5.Rows[7].Cells[0].Value = "单体8电压";
            dataGridView5.Rows[7].Cells[0].Value = "单体8电压";
            dataGridView5.Rows[8].Cells[0].Value = "单体9电压";
            dataGridView5.Rows[9].Cells[0].Value = "单体10电压";
            dataGridView5.Rows[10].Cells[0].Value = "单体11电压";
            dataGridView5.Rows[11].Cells[0].Value = "单体12电压";
            dataGridView5.Rows[12].Cells[0].Value = "单体13电压";
            dataGridView5.Rows[13].Cells[0].Value = "单体14电压";
            dataGridView5.Rows[14].Cells[0].Value = "单体15电压";
            dataGridView5.Rows[15].Cells[0].Value = "电池温度1";
            dataGridView5.Rows[16].Cells[0].Value = "电池温度2";
            dataGridView5.Rows[17].Cells[0].Value = "电池温度3";
            //dataGridView5.Rows[18].Cells[0].Value = "电池温度4";
            //dataGridView5.Rows[19].Cells[0].Value = "电池温度5";
            //dataGridView5.Rows[20].Cells[0].Value = "电池温度6";
            //dataGridView5.Rows[21].Cells[0].Value = "电池温度7";
            //dataGridView5.Rows[22].Cells[0].Value = "电池温度8";
            dataGridView6.Rows[0].Cells[0].Value = "平均电压";
            dataGridView6.Rows[1].Cells[0].Value = "最大电压";
            dataGridView6.Rows[2].Cells[0].Value = "最小电压";
            dataGridView6.Rows[3].Cells[0].Value = "最大电压编号";
            dataGridView6.Rows[4].Cells[0].Value = "最小电压编号";
            dataGridView6.Rows[5].Cells[0].Value = "平均温度";
            dataGridView6.Rows[6].Cells[0].Value = "最高温度";
            dataGridView6.Rows[7].Cells[0].Value = "最低温度";
            dataGridView6.Rows[8].Cells[0].Value = "最高温度编号";
            dataGridView6.Rows[9].Cells[0].Value = "最低温度编号";
            dataGridView6.Rows[10].Cells[0].Value = "电池序列号";
            dataGridView6.Rows[11].Cells[0].Value = "MOS温度";

        }
        #region CAN连接
        private void windowsUIButtonPanel1_Click(object sender, EventArgs e)
        {
            if (m_bOpen == 1)
            {
                VCI_CloseDevice(m_devtype, m_devind);
                m_bOpen = 0;
            }
            else
            {
                if (VCI_OpenDevice(m_devtype, m_devind, 0) == 0)
                {
                    MessageBox.Show("打开设备失败,请检查设备是否连接正确", "错误",
                            MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    return;
                }
                //m_devtype = m_arrdevtype[toolStripComboBox1.SelectedIndex];
                m_devind = (UInt32)CAN索引.SelectedIndex;
                m_canind = (UInt32)CAN通道.SelectedIndex;
                m_bOpen = 1;
                VCI_INIT_CONFIG config = new VCI_INIT_CONFIG();
                config.AccCode = 0;
                config.AccMask = 0xFFFFFFFF;
                config.Timing0 = 0x01;
                config.Timing1 = 0x1C;
                config.Filter = 1;
                config.Mode = 0;
                VCI_InitCAN(m_devtype, m_devind, m_canind, ref config);
                VCI_StartCAN(m_devtype, m_devind, m_canind);
            }           
            if (m_bOpen == 1)
            {
                timer1.Change(0, 100);
                Conn2.Image = Properties.Resources.电脑连接;
                Conn1.Image = Properties.Resources.电脑连接;
                连接pb1.Image = Properties.Resources.系统循环;
                //Thread PCSdisplay = new Thread(new ThreadStart(display));
                //PCSdisplay.IsBackground = true;
                //PCSdisplay.Start();
                //Thread PCSdisplay1 = new Thread(new ThreadStart(display1));
                //PCSdisplay1.IsBackground = true;
                //PCSdisplay1.Start();

                Task t2 = Task.Factory.StartNew(display);
                Task t1 = Task.Factory.StartNew(display1);

                //t1.Start();
                报文 = true;
            }
            else if (m_bOpen == 0)
            {
                timer1.Change(-1, 100);
                Conn2.Image = Properties.Resources.系统连接;
                Conn1.Image = Properties.Resources.系统连接;
                连接pb1.Image = Properties.Resources.循环2;
                运行 = false;
                报文 = false;
            }
        }
        #endregion
        private void intialtimer()
        { timer1 = new System.Threading.Timer(timeaction1, null, Timeout.Infinite, 200); }
        unsafe static void timeaction1(object obj)
        {
            if (intimer)
                return;
            intimer = true;
            VCI_CAN_OBJ sendobj2 = new VCI_CAN_OBJ();
            sendobj2.ExternFlag = 1;
            sendobj2.ID = System.Convert.ToUInt32("18451101", 16);
            sendobj2.DataLen = 8;
            sendobj2.Data[0] = Convert.ToByte(00);
            sendobj2.Data[1] = Convert.ToByte(00);
            sendobj2.Data[2] = Convert.ToByte(00);
            sendobj2.Data[3] = Convert.ToByte(00);
            sendobj2.Data[4] = Convert.ToByte(00);
            sendobj2.Data[5] = Convert.ToByte(00);
            sendobj2.Data[6] = Convert.ToByte(00);
            sendobj2.Data[7] = Convert.ToByte(00);
            if (VCI_Transmit(m_devtype, m_devind, m_canind, ref sendobj2, 1) == 0)
            {
                MessageBox.Show("发送失败", "错误",
                        MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
  
            UInt32 res = new UInt32();
            res = VCI_GetReceiveNum(m_devtype, m_devind, m_canind);
            //if (res == 0)
            //    return;
            UInt32 con_maxlen = 50;
            IntPtr pt = Marshal.AllocHGlobal(Marshal.SizeOf(typeof(VCI_CAN_OBJ)) * (Int32)con_maxlen);
            res = VCI_Receive(m_devtype, m_devind, m_canind, pt, con_maxlen, 100);
            

            for (int i = 0; i < res; i++)
            {
                VCI_CAN_OBJ obj1 = (VCI_CAN_OBJ)Marshal.PtrToStructure((IntPtr)((UInt32)pt + i * Marshal.SizeOf(typeof(VCI_CAN_OBJ))), typeof(VCI_CAN_OBJ));
                if (obj1.ID == Convert.ToUInt32("18080111", 16))
                {
                    v1[0] = Convert.ToUInt32((obj1.Data[1] * 256 + obj1.Data[0]));
                    v1[1] = Convert.ToUInt32((obj1.Data[3] * 256 + obj1.Data[2]));
                    v1[2] = Convert.ToUInt32((obj1.Data[5] * 256 + obj1.Data[4]));
                    v1[3] = Convert.ToUInt32((obj1.Data[7] * 256 + obj1.Data[6]));
                }
                if (obj1.ID == Convert.ToUInt32("18090111", 16))
                {
                    v1[4] = Convert.ToUInt32((obj1.Data[1] * 256 + obj1.Data[0]));
                    v1[5] = Convert.ToUInt32((obj1.Data[3] * 256 + obj1.Data[2]));
                    v1[6] = Convert.ToUInt32((obj1.Data[5] * 256 + obj1.Data[4]));
                    v1[7] = Convert.ToUInt32((obj1.Data[7] * 256 + obj1.Data[6]));
                }
                if (obj1.ID == Convert.ToUInt32("180A0111", 16))
                {
                    v1[8] = Convert.ToUInt32((obj1.Data[1] * 256 + obj1.Data[0]));
                    v1[9] = Convert.ToUInt32((obj1.Data[3] * 256 + obj1.Data[2]));
                    v1[10] = Convert.ToUInt32((obj1.Data[5] * 256 + obj1.Data[4]));
                    v1[11] = Convert.ToUInt32((obj1.Data[7] * 256 + obj1.Data[6]));
                }
                if (obj1.ID == Convert.ToUInt32("180B0111", 16))
                {
                    v1[12] = Convert.ToUInt32((obj1.Data[1] * 256 + obj1.Data[0]));
                    v1[13] = Convert.ToUInt32((obj1.Data[3] * 256 + obj1.Data[2]));
                    v1[14] = Convert.ToUInt32((obj1.Data[5] * 256 + obj1.Data[4]));
                    v1[15] = Convert.ToUInt32((obj1.Data[7] * 256 + obj1.Data[6]));
                }
                if (obj1.ID == Convert.ToUInt32("180C0111", 16))
                {
                    t1[0] = Convert.ToInt32(obj1.Data[0] - 40); t1[1] = Convert.ToInt32((obj1.Data[1]) - 40); t1[2] = Convert.ToInt32((obj1.Data[2]) - 40); t1[3] = Convert.ToInt32((obj1.Data[3]) - 40);
                    t1[4] = Convert.ToInt32((obj1.Data[4]) - 40); t1[5] = Convert.ToInt32((obj1.Data[5]) - 40); t1[6] = Convert.ToInt32((obj1.Data[6]) - 40); t1[7] = Convert.ToInt32((obj1.Data[7]) - 40);
                }
                if (obj1.ID == Convert.ToUInt32("18010111", 16))
                {
                    uint temp1;
                    Pcs变量.额定电压 = Convert.ToUInt32((obj1.Data[1] * 256 + obj1.Data[0]) * 0.1);
                    Pcs变量.额定容量 = Convert.ToUInt32((obj1.Data[3] * 256 + obj1.Data[2]) * 0.1);
                    //Pcs变量.从控节点数量 = Convert.ToUInt32((obj1.Data[4]));
                    temp1 = Convert.ToUInt32((obj1.Data[5]));
                    Pcs变量.电流传感器类型 = Convert.ToUInt32((obj1.Data[6]));
                    Pcs变量.温度传感器类型 = Convert.ToUInt32((obj1.Data[7]));
                    if (temp1 == 1) { Pcs变量.电池类型 = "铅酸电池"; }
                    else if (temp1 == 2) { Pcs变量.电池类型 = "镍氢电池"; }
                    else if (temp1 == 3) { Pcs变量.电池类型 = "磷酸铁锂电池"; }
                    else if (temp1 == 4) { Pcs变量.电池类型 = "锰酸锂电池"; }
                    else if (temp1 == 5) { Pcs变量.电池类型 = "钴酸锂电池"; }
                    else if (temp1 == 6) { Pcs变量.电池类型 = "三元电池"; }
                    else if (temp1 == 8) { Pcs变量.电池类型 = "钛酸锂电池"; }
                    else { Pcs变量.电池类型 = "其他电池"; }
                }
                if (obj1.ID == Convert.ToUInt32("18020111", 16))
                {
                    Pcs变量.单体最低电压 = Convert.ToUInt32((obj1.Data[1] * 256 + obj1.Data[0])) * 0.001;
                    //Pcs变量.单体最低电压所在Pack号 = Convert.ToUInt32((obj1.Data[2]));
                    Pcs变量.单体最低电压所在Pack内编号 = Convert.ToUInt32((obj1.Data[2]));
                    Pcs变量.单体最高电压 = Convert.ToUInt32((obj1.Data[4] * 256 + obj1.Data[3])) * 0.001;
                    //Pcs变量.单体最高电压所在Pack号 = Convert.ToUInt32((obj1.Data[6]));
                    Pcs变量.单体最高电压所在Pack内编号 = Convert.ToUInt32((obj1.Data[5]));
                    AveVoltage = Convert.ToUInt32((obj1.Data[7] * 256 + obj1.Data[6]));
                }
                if (obj1.ID == Convert.ToUInt32("18030111", 16))
                {
                    Pcs变量.最低温度 = Convert.ToInt64((obj1.Data[0])) - 40;
                    //Pcs变量.最低温度所在Pack号 = Convert.ToUInt32((obj1.Data[1]));
                    Pcs变量.最低温度所在Pack内编号 = Convert.ToUInt32((obj1.Data[1]));
                    Pcs变量.最高温度 = Convert.ToInt64((obj1.Data[2])) - 40;
                    //Pcs变量.最高温度所在Pack号 = Convert.ToUInt32((obj1.Data[4]));
                    Pcs变量.最高温度所在Pack内编号 = Convert.ToUInt32((obj1.Data[3]));
                    AveTemperture = Convert.ToInt32(Convert.ToInt32(obj1.Data[4]) - 40);
                    Pcs变量.电压个数 = Convert.ToUInt32((obj1.Data[5]));
                    Pcs变量.电压个数 = Convert.ToUInt32((obj1.Data[6]));
                    Pcs变量.MOS温度= Convert.ToInt32(Convert.ToInt32(obj1.Data[7]) - 40);
                }
                if (obj1.ID == Convert.ToUInt32("18040111", 16))
                {
                    Pcs变量.总电压 = Convert.ToUInt32(Convert.ToUInt32(obj1.Data[1] * 256 + obj1.Data[0])) * 0.1;
                    Pcs变量.实时电流 = Convert.ToInt64(Convert.ToInt64(obj1.Data[3] * 256 + obj1.Data[2]) - 32000) * 0.1;
                    Pcs变量.SOC = Convert.ToUInt32(Convert.ToUInt32(obj1.Data[4]) * 0.4);
                    Pcs变量.SOH = Convert.ToUInt32(Convert.ToUInt32(obj1.Data[5]) * 0.4);
                    Pcs变量.BMS充电次数 = Convert.ToUInt32(Convert.ToUInt32(obj1.Data[7] * 256 + obj1.Data[6]));
                }
                if (obj1.ID == Convert.ToUInt32("18050111", 16))
                {
                    Pcs变量.最大允许充电电压 = Convert.ToUInt32(Convert.ToUInt32(obj1.Data[1] * 256 + obj1.Data[0]) * 0.1);
                    Pcs变量.最小允许放电电压 = Convert.ToUInt32(Convert.ToUInt32(obj1.Data[3] * 256 + obj1.Data[2]) * 0.1);
                    Pcs变量.最大允许充电电流 = Convert.ToUInt32(Convert.ToUInt32(obj1.Data[5] * 256 + obj1.Data[4]) * 0.1);
                    Pcs变量.最大允许放电电流 = Convert.ToUInt32(Convert.ToUInt32(obj1.Data[7] * 256 + obj1.Data[6]) * 0.1);
                }
                if (obj1.ID == Convert.ToUInt32("18070111", 16))
                {
                    uint temp1, temp2, temp3, temp4, temp5, temp6, temp7,temp8;
                    temp1 = Convert.ToUInt32((obj1.Data[0]));
                    string tempstr1 = DecimalToBinary1(temp1);
                    tempstr1 = string.Format("{0:d8}", Convert.ToInt32(tempstr1));
                    string str1 = tempstr1; str1 = str1.Substring(4, 2); /*string str2 = tempstr1; str2 = str2.Substring(4, 2);*/ string str3 = tempstr1; str3 = str3.Substring(6, 2);
                    if (str1 == "00") { Pcs变量.BMS工作状态 = "等待充放电"; }
                    else if (str1 == "01") { Pcs变量.BMS工作状态 = "充电"; }
                    else if (str1 == "10") { Pcs变量.BMS工作状态 = "放电"; }
                    else if (str1 == "11") { Pcs变量.BMS工作状态 = "故障停车"; }
                    //else if (str1 == "1011") { Pcs变量.BMS工作状态 = "放电"; }
                    //else if (str1 == "1111") { Pcs变量.BMS工作状态 = "等待充放电"; }
                    //else { Pcs变量.BMS工作状态 = "无"; }
                    //if (str2 == "00") { Pcs变量.Ready = "BMS没准备好"; }
                    //if (str2 == "01") { Pcs变量.Ready = "BMS已经准备好"; }
                    if (str3 == "00") { Pcs变量.BMS最高告警故障状态 = "无告警"; }
                    if (str3 == "01") { Pcs变量.BMS最高告警故障状态 = "一级告警"; }
                    if (str3 == "10") { Pcs变量.BMS最高告警故障状态 = "二级告警"; }
                    if (str3 == "11") { Pcs变量.BMS最高告警故障状态 = "三级告警"; }
                    temp2 = Convert.ToUInt32((obj1.Data[1]));
                    string tempstr2 = DecimalToBinary1(temp2);
                    tempstr2 = string.Format("{0:d8}", Convert.ToInt32(tempstr2));
                    /*string str4 = tempstr2; str4 = str4.Substring(0, 1);*/ /*string str5 = tempstr2; str5 = str5.Substring(1, 1);*/ string str6 = tempstr2; str6 = str6.Substring(2, 1);
                    string str7 = tempstr2; str7 = str7.Substring(3, 1); string str8 = tempstr2; str8 = str8.Substring(4, 1); string str9 = tempstr2; str9 = str9.Substring(5, 1);
                    string str10 = tempstr2; str10 = str10.Substring(6, 1); string str11 = tempstr2; str11 = str11.Substring(7, 1);
                    //if (str4 == "0") { Pcs变量.BMU8链接故障 = "无故障"; }
                    //if (str4 == "1") { Pcs变量.BMU8链接故障 = "连接故障"; }
                    //if (str5 == "0") { Pcs变量.BMU7链接故障 = "无故障"; }
                    //if (str5 == "1") { Pcs变量.BMU7链接故障 = "连接故障"; }
                    if (str6 == "0") { Pcs变量.BMU电压故障 = "无故障"; }
                    if (str6 == "1") { Pcs变量.BMU电压故障 = "连接故障"; }
                    if (str7 == "0") { Pcs变量.温度传感器故障 = "无故障"; }
                    if (str7 == "1") { Pcs变量.温度传感器故障 = "连接故障"; }
                    if (str8 == "0") { Pcs变量.电流传感器故障 = "无故障"; }
                    if (str8 == "1") { Pcs变量.电流传感器故障 = "连接故障"; }
                    if (str9 == "0") { Pcs变量.绝缘模块连接故障 = "无故障"; }
                    if (str9 == "1") { Pcs变量.绝缘模块连接故障 = "连接故障"; }
                    if (str10 == "0") { Pcs变量.放电接触器状态 = "断开"; }
                    if (str10 == "1") { Pcs变量.放电接触器状态 = "闭合"; }
                    if (str11 == "0") { Pcs变量.充电接触器状态 = "断开"; }
                    if (str11 == "1") { Pcs变量.充电接触器状态 = "闭合"; }
                    temp3 = Convert.ToUInt32((obj1.Data[2]));
                    string tempstr3 = DecimalToBinary1(temp3);
                    tempstr3 = string.Format("{0:d8}", Convert.ToInt32(tempstr3));
                    string str34 = tempstr3; str34 = str34.Substring(0, 1);
                    string str12 = tempstr3; str12 = str12.Substring(1, 1); string str13 = tempstr3; str13 = str13.Substring(2, 1); string str14 = tempstr3; str14 = str14.Substring(3, 1);
                    string str35 = tempstr3; str35 = str35.Substring(4, 1); string str36 = tempstr3; str36 = str36.Substring(5, 1);
                    string str15 = tempstr3; str15 = str15.Substring(6, 1); string str16 = tempstr3; str16 = str16.Substring(7, 1);
                    if (str16 == "0") { Pcs变量.轻微漏电 = "无故障"; }
                    if (str16 == "1") { Pcs变量.轻微漏电 = "轻微故障"; }
                    if (str15 == "0") { Pcs变量.轻微单体过高 = "无故障"; }
                    if (str15 == "1") { Pcs变量.轻微单体过高 = "轻微故障"; }
                    if (str36 == "0") { Pcs变量.轻微单体过低 = "无故障"; }
                    if (str36 == "1") { Pcs变量.轻微单体过低 = "轻微故障"; }
                    if (str35 == "0") { Pcs变量.轻微总压过高 = "无故障"; }
                    if (str35 == "1") { Pcs变量.轻微总压过高 = "轻微故障"; }
                    if (str14 == "0") { Pcs变量.轻微总压过低 = "无故障"; }
                    if (str14 == "1") { Pcs变量.轻微总压过低 = "轻微故障"; }
                    if (str13 == "0") { Pcs变量.轻微MOS温度过高 = "无故障"; }
                    if (str13 == "1") { Pcs变量.轻微MOS温度过高 = "轻微故障"; }
                    if (str12 == "0") { Pcs变量.轻微充电电流过高 = "无故障"; }
                    if (str12 == "1") { Pcs变量.轻微充电电流过高 = "轻微故障"; }
                    if (str34 == "0") { Pcs变量.轻微放电电流过高 = "无故障"; }
                    if (str34 == "1") { Pcs变量.轻微放电电流过高 = "轻微故障"; }
                    temp4 = Convert.ToUInt32((obj1.Data[3]));
                    string tempstr4 = DecimalToBinary1(temp4);
                    tempstr4 = string.Format("{0:d8}", Convert.ToInt32(tempstr4));
                    string str17 = tempstr4; str17 = str17.Substring(3, 1); string str18 = tempstr4; str18 = str18.Substring(4, 1);
                    string str19 = tempstr4; str19 = str19.Substring(5, 1); string str20 = tempstr4; str20 = str20.Substring(6, 1);
                    string str21 = tempstr4; str21 = str21.Substring(7, 1); string str37 = tempstr4; str37 = str37.Substring(2, 1);
                    string str38 = tempstr4; str38 = str38.Substring(1, 1); string str39 = tempstr4; str39 = str39.Substring(0, 1);
                    if (str17 == "0") { Pcs变量.轻微单体压差过大 = "无故障"; }
                    if (str17 == "1") { Pcs变量.轻微单体压差过大 = "轻微故障"; }
                    if (str18 == "0") { Pcs变量.轻微放电温度过低 = "无故障"; }
                    if (str18 == "1") { Pcs变量.轻微放电温度过低 = "轻微故障"; }
                    if (str19 == "0") { Pcs变量.轻微放电温度过高 = "无故障"; }
                    if (str19 == "1") { Pcs变量.轻微放电温度过高 = "轻微故障"; }
                    if (str20 == "0") { Pcs变量.轻微充电温度过低 = "无故障"; }
                    if (str20 == "1") { Pcs变量.轻微充电温度过低 = "轻微故障"; }
                    if (str21 == "0") { Pcs变量.轻微充电温度过高 = "无故障"; }
                    if (str21 == "1") { Pcs变量.轻微充电温度过高 = "轻微故障"; }
                    if (str37 == "0") { Pcs变量.轻微温差过大 = "无故障"; }
                    if (str37 == "1") { Pcs变量.轻微温差过大 = "轻微故障"; }
                    if (str38 == "0") { Pcs变量.轻微SOC过低 = "无故障"; }
                    if (str38 == "1") { Pcs变量.轻微SOC过低 = "轻微故障"; }
                    if (str39 == "0") { Pcs变量.轻微SOC过高 = "无故障"; }
                    if (str39 == "1") { Pcs变量.轻微SOC过高 = "轻微故障"; }
                    temp5 = Convert.ToUInt32((obj1.Data[4]));
                    string tempstr5 = DecimalToBinary1(temp5);
                    tempstr5 = string.Format("{0:d8}", Convert.ToInt32(tempstr5));
                    string str22 = tempstr5; str22 = str22.Substring(0, 1); string str23 = tempstr5; str23 = str23.Substring(1, 1);
                    string str24 = tempstr5; str24 = str24.Substring(2, 1); string str25 = tempstr5; str25 = str25.Substring(3, 1);
                    string str40 = tempstr5; str40 = str40.Substring(4, 1); string str41 = tempstr5; str41 = str41.Substring(5, 1);
                    string str42 = tempstr5; str42 = str42.Substring(6, 1); string str43 = tempstr5; str43 = str43.Substring(7, 1);
                    if (str22 == "0") { Pcs变量.一般放电电流过高 = "无告警"; }
                    if (str22 == "1") { Pcs变量.一般放电电流过高 = "一般告警"; }
                    if (str23 == "0") { Pcs变量.一般充电电流过高 = "无告警"; }
                    if (str23 == "1") { Pcs变量.一般充电电流过高 = "一般告警"; }
                    if (str24 == "0") { Pcs变量.一般MOS温度过高 = "无告警"; }
                    if (str24 == "1") { Pcs变量.一般MOS温度过高 = "一般告警"; }
                    if (str25 == "0") { Pcs变量.一般总压过低 = "无告警"; }
                    if (str25 == "1") { Pcs变量.一般总压过低 = "一般告警"; }
                    if (str40 == "0") { Pcs变量.一般总压过高 = "无告警"; }
                    if (str40 == "1") { Pcs变量.一般总压过高 = "一般告警"; }
                    if (str41 == "0") { Pcs变量.一般单体过低 = "无告警"; }
                    if (str41 == "1") { Pcs变量.一般单体过低 = "一般告警"; }
                    if (str42 == "0") { Pcs变量.一般单体过高 = "无告警"; }
                    if (str42 == "1") { Pcs变量.一般单体过高 = "一般告警"; }
                    if (str43 == "0") { Pcs变量.一般漏电 = "无告警"; }
                    if (str43 == "1") { Pcs变量.一般漏电 = "一般告警"; }
                    temp6 = Convert.ToUInt32((obj1.Data[5]));
                    string tempstr6 = DecimalToBinary1(temp6);
                    tempstr6 = string.Format("{0:d8}", Convert.ToInt32(tempstr6));
                    string str26 = tempstr6; str26 = str26.Substring(0, 1); string str27 = tempstr6; str27 = str27.Substring(1, 1);
                    string str28 = tempstr6; str28 = str28.Substring(2, 1); string str29 = tempstr6; str29 = str29.Substring(3, 1);
                    string str44 = tempstr6; str44 = str44.Substring(4, 1); string str45 = tempstr6; str45 = str45.Substring(5, 1);
                    string str46 = tempstr6; str46 = str46.Substring(6, 1); string str47 = tempstr6; str47 = str47.Substring(7, 1);
                    if (str26 == "0") { Pcs变量.一般SOC过高 = "无告警"; }
                    if (str26 == "1") { Pcs变量.一般SOC过高 = "一般告警"; }
                    if (str27 == "0") { Pcs变量.一般SOC过低 = "无告警"; }
                    if (str27 == "1") { Pcs变量.一般SOC过低 = "一般告警"; }
                    if (str28 == "0") { Pcs变量.一般温差过大 = "无告警"; }
                    if (str28 == "1") { Pcs变量.一般温差过大 = "一般告警"; }
                    if (str29 == "0") { Pcs变量.一般单体压差过大 = "无告警"; }
                    if (str29 == "1") { Pcs变量.一般单体压差过大 = "一般告警"; }
                    if (str44 == "0") { Pcs变量.一般放电温度过低 = "无告警"; }
                    if (str44 == "1") { Pcs变量.一般放电温度过低 = "一般告警"; }
                    if (str45 == "0") { Pcs变量.一般放电温度过高 = "无告警"; }
                    if (str45 == "1") { Pcs变量.一般放电温度过高 = "一般告警"; }
                    if (str46 == "0") { Pcs变量.一般充电温度过低 = "无告警"; }
                    if (str46 == "1") { Pcs变量.一般充电温度过低 = "一般告警"; }
                    if (str47 == "0") { Pcs变量.一般充电温度过高 = "无告警"; }
                    if (str47 == "1") { Pcs变量.一般充电温度过高 = "一般告警"; }
                    temp7 = Convert.ToUInt32((obj1.Data[6]));
                    string tempstr7 = DecimalToBinary1(temp7);
                    tempstr7 = string.Format("{0:d8}", Convert.ToInt32(tempstr7));
                    string str30 = tempstr7; str30 = str30.Substring(0, 1); string str31 = tempstr7; str31 = str31.Substring(1, 1);
                    string str32 = tempstr7; str32 = str32.Substring(2, 1); string str33 = tempstr7; str33 = str33.Substring(3, 1);
                    string str48 = tempstr7; str48 = str48.Substring(4, 1); string str49 = tempstr7; str49 = str49.Substring(5, 1);
                    string str50 = tempstr7; str50 = str50.Substring(6, 1); string str51 = tempstr7; str51 = str51.Substring(7, 1);
                    if (str30 == "0") { Pcs变量.严重放电电流过高 = "无告警"; }
                    if (str30 == "1") { Pcs变量.严重放电电流过高 = "严重告警"; }
                    if (str31 == "0") { Pcs变量.严重充电电流过高 = "无告警"; }
                    if (str31 == "1") { Pcs变量.严重充电电流过高 = "严重告警"; }
                    if (str32 == "0") { Pcs变量.严重MOS温度过高 = "无告警"; }
                    if (str32 == "1") { Pcs变量.严重MOS温度过高 = "严重告警"; }
                    if (str33 == "0") { Pcs变量.严重总压过低 = "无告警"; }
                    if (str33 == "1") { Pcs变量.严重总压过低 = "严重告警"; }
                    if (str48 == "0") { Pcs变量.严重总压过高 = "无告警"; }
                    if (str48 == "1") { Pcs变量.严重总压过高 = "严重告警"; }
                    if (str49 == "0") { Pcs变量.严重单体过低 = "无告警"; }
                    if (str49 == "1") { Pcs变量.严重单体过低 = "严重告警"; }
                    if (str50 == "0") { Pcs变量.严重单体过高 = "无告警"; }
                    if (str50 == "1") { Pcs变量.严重单体过高 = "严重告警"; }
                    if (str51 == "0") { Pcs变量.严重漏电 = "无告警"; }
                    if (str51 == "1") { Pcs变量.严重漏电 = "严重告警"; }
                    temp8 = Convert.ToUInt32((obj1.Data[7]));
                    string tempstr8 = DecimalToBinary1(temp8);
                    tempstr8= string.Format("{0:d8}", Convert.ToInt32(tempstr8));
                    string str52 = tempstr8; str52 = str52.Substring(0, 1); string str53 = tempstr8; str53 = str53.Substring(1, 1);
                    string str54 = tempstr8; str54 = str54.Substring(2, 1); string str55 = tempstr8; str55 = str55.Substring(3, 1);
                    string str56 = tempstr8; str56 = str56.Substring(4, 1); string str57 = tempstr8; str57 = str57.Substring(5, 1);
                    string str58 = tempstr8; str58 = str58.Substring(6, 1); string str59 = tempstr8; str59 = str59.Substring(7, 1);
                    if (str52 == "0") { Pcs变量.严重SOC过高 = "无告警"; }
                    if (str52 == "1") { Pcs变量.严重SOC过高 = "严重告警"; }
                    if (str53 == "0") { Pcs变量.严重SOC过低 = "无告警"; }
                    if (str53 == "1") { Pcs变量.严重SOC过低 = "严重告警"; }
                    if (str54 == "0") { Pcs变量.严重温差过大 = "无告警"; }
                    if (str54 == "1") { Pcs变量.严重温差过大 = "严重告警"; }
                    if (str55 == "0") { Pcs变量.严重单体压差过大 = "无告警"; }
                    if (str55 == "1") { Pcs变量.严重单体压差过大 = "严重告警"; }
                    if (str56 == "0") { Pcs变量.严重放电温度过低 = "无告警"; }
                    if (str56 == "1") { Pcs变量.严重放电温度过低 = "严重告警"; }
                    if (str57 == "0") { Pcs变量.严重放电温度过高 = "无告警"; }
                    if (str57 == "1") { Pcs变量.严重放电温度过高 = "严重告警"; }
                    if (str58 == "0") { Pcs变量.严重充电温度过低 = "无告警"; }
                    if (str58 == "1") { Pcs变量.严重充电温度过低 = "严重告警"; }
                    if (str59 == "0") { Pcs变量.严重充电温度过高 = "无告警"; }
                    if (str59 == "1") { Pcs变量.严重充电温度过高 = "严重告警"; }
                }
                if (obj1.ID == Convert.ToUInt32("18060111", 16))
                {
                    Pcs变量.电池序列号 = Convert.ToUInt32(obj1.Data[1] * 256 + obj1.Data[0]);
                }
              
                switch (obj1.ID)
                {
                    case 0x18010111:                       
                                        
                        运行 = true;
                        break;                 
                    case 0x180029f3:
                        if (Convert.ToUInt32(obj1.Data[0]) == 135)
                        {
                            temp[0] = Convert.ToString((Convert.ToDouble(obj1.Data[2])) / 2.5);
                        }
                        if (Convert.ToUInt32(obj1.Data[0]) == 191)
                        {
                            temp[1] = Convert.ToString((Convert.ToDouble(obj1.Data[2])) / 2.5);
                        }
                        if (Convert.ToUInt32(obj1.Data[0]) == 136)
                        {
                            temp[2] = Convert.ToString((Convert.ToUInt32(obj1.Data[2])));
                        }
                        if (Convert.ToUInt32(obj1.Data[0]) == 137)
                        {
                            temp[3] = Convert.ToString((Convert.ToUInt32(obj1.Data[2])));
                        }
                        if (Convert.ToUInt32(obj1.Data[0]) == 138)
                        {
                            temp[4] = Convert.ToString((Convert.ToUInt32(obj1.Data[2])));
                        }
                        if (Convert.ToUInt32(obj1.Data[0]) == 139)
                        {
                            temp[5] = Convert.ToString((Convert.ToUInt32(obj1.Data[2])));
                        }
                        if (Convert.ToUInt32(obj1.Data[0]) == 01)
                        {
                            temp[6] = Convert.ToString((Convert.ToUInt32(obj1.Data[2])));
                        }
                        if (Convert.ToUInt32(obj1.Data[0]) == 81)
                        {
                            tempv[0] = Convert.ToUInt32(obj1.Data[2]);
                        }
                        if (Convert.ToUInt32(obj1.Data[0]) == 82)
                        {
                            tempv[1] = Convert.ToUInt32(obj1.Data[2]) * 256;
                            temp[7] = Convert.ToString(Convert.ToSingle(tempv[0] + tempv[1]) / 1000);
                        }
                        if (Convert.ToUInt32(obj1.Data[0]) == 83)
                        {
                            tempv[2] = Convert.ToUInt32(obj1.Data[2]);
                        }
                        if (Convert.ToUInt32(obj1.Data[0]) == 84)
                        {
                            tempv[3] = Convert.ToUInt32(obj1.Data[2]) * 256;
                            temp[8] = Convert.ToString(Convert.ToSingle(tempv[3] + tempv[2]) / 1000);
                        }
                        if (Convert.ToUInt32(obj1.Data[0]) == 85)
                        {
                            tempv[4] = Convert.ToUInt32(obj1.Data[2]);
                        }
                        if (Convert.ToUInt32(obj1.Data[0]) == 86)
                        {
                            tempv[5] = Convert.ToUInt32(obj1.Data[2]) * 256;
                            temp[9] = Convert.ToString(Convert.ToSingle(tempv[5] + tempv[4]) / 1000);
                        }
                        if (Convert.ToUInt32(obj1.Data[0]) == 87)
                        {
                            tempv[6] = Convert.ToUInt32(obj1.Data[2]);
                        }
                        if (Convert.ToUInt32(obj1.Data[0]) == 88)
                        {
                            tempv[7] = Convert.ToUInt32(obj1.Data[2]) * 256;
                            temp[10] = Convert.ToString(Convert.ToSingle(tempv[6] + tempv[7]) / 1000);
                        }
                        if (Convert.ToUInt32(obj1.Data[0]) == 89)
                        {
                            tempv[8] = Convert.ToUInt32(obj1.Data[2]);
                        }
                        if (Convert.ToUInt32(obj1.Data[0]) == 90)
                        {
                            tempv[9] = Convert.ToUInt32(obj1.Data[2]) * 256;
                            temp[11] = Convert.ToString(Convert.ToSingle(tempv[8] + tempv[9]) / 1000);
                        }
                        if (Convert.ToUInt32(obj1.Data[0]) == 91)
                        {
                            tempv[10] = Convert.ToUInt32(obj1.Data[2]);
                        }
                        if (Convert.ToUInt32(obj1.Data[0]) == 92)
                        {
                            tempv[11] = Convert.ToUInt32(obj1.Data[2]) * 256;
                            temp[12] = Convert.ToString(Convert.ToSingle(tempv[10] + tempv[11]) / 1000);
                        }
                        if (Convert.ToUInt32(obj1.Data[0]) == 31)
                        {
                            temp[13] = Convert.ToString(Convert.ToUInt32(obj1.Data[2]));
                        }
                        if (Convert.ToUInt32(obj1.Data[0]) == 140)
                        {
                            tempv[12] = Convert.ToUInt32(obj1.Data[2]);
                        }
                        if (Convert.ToUInt32(obj1.Data[0]) == 141)
                        {
                            tempv[13] = Convert.ToUInt32(obj1.Data[2]) * 256;
                            temp[14] = Convert.ToString(Convert.ToSingle(tempv[13] + tempv[12]) / 1000);
                        }
                        if (Convert.ToUInt32(obj1.Data[0]) == 21)
                        {
                            temp[15] = Convert.ToString(Convert.ToUInt32(obj1.Data[2]));
                        }
                        if (Convert.ToUInt32(obj1.Data[0]) == 25)
                        {
                            temp[16] = Convert.ToString(Convert.ToInt64((Convert.ToInt32(obj1.Data[2])) - 40));
                        }
                        if (Convert.ToUInt32(obj1.Data[0]) == 26)
                        {
                            temp[17] = Convert.ToString(Convert.ToInt64((Convert.ToInt32(obj1.Data[2])) - 40));
                        }
                        if (Convert.ToUInt32(obj1.Data[0]) == 27)
                        {
                            temp[18] = Convert.ToString(Convert.ToInt64((Convert.ToInt32(obj1.Data[2])) - 40));
                        }
                        if (Convert.ToUInt32(obj1.Data[0]) == 28)
                        {
                            temp[19] = Convert.ToString(Convert.ToInt64((Convert.ToInt32(obj1.Data[2])) - 40));
                        }
                        if (Convert.ToUInt32(obj1.Data[0]) == 29)
                        {
                            temp[20] = Convert.ToString(Convert.ToInt64((Convert.ToInt32(obj1.Data[2])) - 40));
                        }
                        if (Convert.ToUInt32(obj1.Data[0]) == 30)
                        {
                            temp[21] = Convert.ToString(Convert.ToInt64((Convert.ToInt32(obj1.Data[2])) - 40));
                        }
                        if (Convert.ToUInt32(obj1.Data[0]) == 190)
                        {
                            temp[22] = Convert.ToString(Convert.ToUInt32(obj1.Data[2]));
                            if (temp[22] == "0") { MessageBox.Show("清除完成！", "标定", MessageBoxButtons.OK, MessageBoxIcon.Information); }
                        }
                        if (Convert.ToUInt32(obj1.Data[0]) == 93)
                        {
                            tempv[15] = Convert.ToUInt32(obj1.Data[2]);
                        }
                        if (Convert.ToUInt32(obj1.Data[0]) == 94)
                        {
                            tempv[16] = Convert.ToUInt32(obj1.Data[2]) * 256;
                            temp[23] = Convert.ToString(Convert.ToInt64(Convert.ToInt64(tempv[15] + tempv[16]) - 32000)/10);
                        }
                        if (Convert.ToUInt32(obj1.Data[0]) == 95)
                        {
                            tempv[17] = Convert.ToUInt32(obj1.Data[2]);
                        }
                        if (Convert.ToUInt32(obj1.Data[0]) == 96)
                        {
                            tempv[18] = Convert.ToUInt32(obj1.Data[2]) * 256;
                            temp[24] = Convert.ToString(Convert.ToInt64(Convert.ToInt64(tempv[17] + tempv[18]) - 32000) / 10);
                        }
                        if (Convert.ToUInt32(obj1.Data[0]) == 97)
                        {
                            tempv[19] = Convert.ToUInt32(obj1.Data[2]);
                        }
                        if (Convert.ToUInt32(obj1.Data[0]) == 98)
                        {
                            tempv[20] = Convert.ToUInt32(obj1.Data[2]) * 256;
                            temp[25] = Convert.ToString(Convert.ToInt64(Convert.ToInt64(tempv[19] + tempv[20]) - 32000) / 10);
                        }
                        if (Convert.ToUInt32(obj1.Data[0]) == 99)
                        {
                            tempv[21] = Convert.ToUInt32(obj1.Data[2]);
                        }
                        if (Convert.ToUInt32(obj1.Data[0]) == 100)
                        {
                            tempv[22] = Convert.ToUInt32(obj1.Data[2]) * 256;
                            temp[26] = Convert.ToString(Convert.ToInt64(Convert.ToInt64(tempv[21] + tempv[22]) - 32000) / 10);
                        }
                        if (Convert.ToUInt32(obj1.Data[0]) == 101)
                        {
                            tempv[23] = Convert.ToUInt32(obj1.Data[2]);
                        }
                        if (Convert.ToUInt32(obj1.Data[0]) == 102)
                        {
                            tempv[24] = Convert.ToUInt32(obj1.Data[2]) * 256;
                            temp[27] = Convert.ToString(Convert.ToInt64(Convert.ToInt64(tempv[23] + tempv[24]) - 32000) / 10);
                        }
                        if (Convert.ToUInt32(obj1.Data[0]) == 103)
                        {
                            tempv[25] = Convert.ToUInt32(obj1.Data[2]);
                        }
                        if (Convert.ToUInt32(obj1.Data[0]) == 104)
                        {
                            tempv[26] = Convert.ToUInt32(obj1.Data[2]) * 256;
                            temp[28] = Convert.ToString(Convert.ToInt64(Convert.ToInt64(tempv[25] + tempv[26]) - 32000) / 10);
                        }
                        if (Convert.ToUInt32(obj1.Data[0]) == 105)
                        {
                            tempv[27] = Convert.ToUInt32(obj1.Data[2]);
                        }
                        if (Convert.ToUInt32(obj1.Data[0]) == 106)
                        {
                            tempv[28] = Convert.ToUInt32(obj1.Data[2]) * 256;
                            temp[29] = Convert.ToString((Convert.ToUInt32(tempv[27] + tempv[28]) ) );
                        }
                        if (Convert.ToUInt32(obj1.Data[0]) == 107)
                        {
                            tempv[29] = Convert.ToUInt32(obj1.Data[2]);
                        }
                        if (Convert.ToUInt32(obj1.Data[0]) == 108)
                        {
                            tempv[30] = Convert.ToUInt32(obj1.Data[2]) * 256;
                            temp[30] = Convert.ToString((Convert.ToUInt32(tempv[29] + tempv[30])));
                        }
                        if (Convert.ToUInt32(obj1.Data[0]) == 109)
                        {
                            tempv[31] = Convert.ToUInt32(obj1.Data[2]);
                        }
                        if (Convert.ToUInt32(obj1.Data[0]) == 110)
                        {
                            tempv[32] = Convert.ToUInt32(obj1.Data[2]) * 256;
                            temp[31] = Convert.ToString((Convert.ToUInt32(tempv[31] + tempv[32])));
                        }
                        if (Convert.ToUInt32(obj1.Data[0]) == 111)
                        {
                            temp[32] = Convert.ToString(Convert.ToInt64((Convert.ToInt32(obj1.Data[2]))));
                        }
                        if (Convert.ToUInt32(obj1.Data[0]) == 112)
                        {
                            temp[33] = Convert.ToString(Convert.ToInt64((Convert.ToInt32(obj1.Data[2])) ));
                        }
                        if (Convert.ToUInt32(obj1.Data[0]) == 113)
                        {
                            temp[34] = Convert.ToString(Convert.ToInt64((Convert.ToInt32(obj1.Data[2])) ));
                        }
                        if (Convert.ToUInt32(obj1.Data[0]) == 114)
                        {
                            temp[35] = Convert.ToString(Convert.ToUInt32(obj1.Data[2]) /2.5);
                        }
                        if (Convert.ToUInt32(obj1.Data[0]) == 115)
                        {
                            temp[36] = Convert.ToString(Convert.ToUInt32(obj1.Data[2]) /2.5);
                        }
                        if (Convert.ToUInt32(obj1.Data[0]) == 116)
                        {
                            temp[37] = Convert.ToString(Convert.ToUInt32(obj1.Data[2]) /2.5);
                        }
                        if (Convert.ToUInt32(obj1.Data[0]) == 238)
                        {
                            temp[38] = Convert.ToString(Convert.ToInt64((Convert.ToInt32(obj1.Data[2])) - 40));
                        }
                        if (Convert.ToUInt32(obj1.Data[0]) == 239)
                        {
                            temp[39] = Convert.ToString(Convert.ToInt64((Convert.ToInt32(obj1.Data[2])) - 40));
                        }
                        if (Convert.ToUInt32(obj1.Data[0]) == 240)
                        {
                            temp[40] = Convert.ToString(Convert.ToInt64((Convert.ToInt32(obj1.Data[2])) - 40));
                        }
                        if (Convert.ToUInt32(obj1.Data[0]) == 22)
                        {
                            temp[41] = Convert.ToString(Convert.ToInt64((Convert.ToInt32(obj1.Data[2])) - 40));
                        }
                        if (Convert.ToUInt32(obj1.Data[0]) == 23)
                        {
                            temp[42] = Convert.ToString(Convert.ToInt64((Convert.ToInt32(obj1.Data[2])) - 40));
                        }
                        if (Convert.ToUInt32(obj1.Data[0]) == 24)
                        {
                            temp[43] = Convert.ToString(Convert.ToInt64((Convert.ToInt32(obj1.Data[2])) - 40));
                        }
                        if (Convert.ToUInt32(obj1.Data[0]) == 241)
                        {
                            temp[44] = Convert.ToString(Convert.ToInt64((Convert.ToInt32(obj1.Data[2])) - 40));
                        }
                        if (Convert.ToUInt32(obj1.Data[0]) == 242)
                        {
                            temp[45] = Convert.ToString(Convert.ToInt64((Convert.ToInt32(obj1.Data[2])) - 40));
                        }
                        if (Convert.ToUInt32(obj1.Data[0]) == 243)
                        {
                            temp[46] = Convert.ToString(Convert.ToInt64((Convert.ToInt32(obj1.Data[2])) - 40));
                        }
                        if (Convert.ToUInt32(obj1.Data[0]) == 244)
                        {
                            temp[47] = Convert.ToString(Convert.ToInt64((Convert.ToInt32(obj1.Data[2])) - 40));
                        }
                        if (Convert.ToUInt32(obj1.Data[0]) == 245)
                        {
                            temp[48] = Convert.ToString(Convert.ToInt64((Convert.ToInt32(obj1.Data[2])) - 40));
                        }
                        if (Convert.ToUInt32(obj1.Data[0]) == 246)
                        {
                            temp[49] = Convert.ToString(Convert.ToInt64((Convert.ToInt32(obj1.Data[2])) - 40));
                        }
                        if (Convert.ToUInt32(obj1.Data[0]) == 160)
                        {
                            temp[50] = Convert.ToString(Convert.ToDouble((Convert.ToDouble(obj1.Data[2])) /10));
                        }
                        if (Convert.ToUInt32(obj1.Data[0]) == 161)
                        {
                            temp[51] = Convert.ToString(Convert.ToDouble((Convert.ToDouble(obj1.Data[2])) / 10));
                        }
                        if (Convert.ToUInt32(obj1.Data[0]) == 162)
                        {
                            temp[52] = Convert.ToString(Convert.ToDouble((Convert.ToDouble(obj1.Data[2])) / 10));
                        }
                        if (Convert.ToUInt32(obj1.Data[0]) == 163)
                        {
                            temp[53] = Convert.ToString(Convert.ToDouble((Convert.ToDouble(obj1.Data[2])) / 10));
                        }
                        if (Convert.ToUInt32(obj1.Data[0]) == 164)
                        {
                            temp[54] = Convert.ToString(Convert.ToDouble((Convert.ToDouble(obj1.Data[2])) / 10));
                        }
                        if (Convert.ToUInt32(obj1.Data[0]) == 165)
                        {
                            temp[55] = Convert.ToString(Convert.ToDouble((Convert.ToDouble(obj1.Data[2])) / 10));
                        }
                        if (Convert.ToUInt32(obj1.Data[0]) == 166)
                        {
                            temp[56] = Convert.ToString(Convert.ToDouble((Convert.ToDouble(obj1.Data[2])) / 10));
                        }
                        if (Convert.ToUInt32(obj1.Data[0]) == 167)
                        {
                            temp[57] = Convert.ToString(Convert.ToDouble((Convert.ToDouble(obj1.Data[2])) / 10));
                        }
                        if (Convert.ToUInt32(obj1.Data[0]) == 168)
                        {
                            temp[58] = Convert.ToString(Convert.ToDouble((Convert.ToDouble(obj1.Data[2])) / 10));
                        }
                        if (Convert.ToUInt32(obj1.Data[0]) == 169)
                        {
                            temp[59] = Convert.ToString(Convert.ToDouble((Convert.ToDouble(obj1.Data[2])) / 10));
                        }
                        if (Convert.ToUInt32(obj1.Data[0]) == 170)
                        {
                            temp[60] = Convert.ToString(Convert.ToDouble((Convert.ToDouble(obj1.Data[2])) / 10));
                        }
                        if (Convert.ToUInt32(obj1.Data[0]) == 171)
                        {
                            temp[61] = Convert.ToString(Convert.ToDouble((Convert.ToDouble(obj1.Data[2])) / 10));
                        }
                        if (Convert.ToUInt32(obj1.Data[0]) == 172)
                        {
                            temp[62] = Convert.ToString(Convert.ToDouble((Convert.ToDouble(obj1.Data[2])) / 10));
                        }
                        if (Convert.ToUInt32(obj1.Data[0]) == 173)
                        {
                            temp[63] = Convert.ToString(Convert.ToDouble((Convert.ToDouble(obj1.Data[2])) / 10));
                        }
                        if (Convert.ToUInt32(obj1.Data[0]) == 174)
                        {
                            temp[64] = Convert.ToString(Convert.ToDouble((Convert.ToDouble(obj1.Data[2])) / 10));
                        }
                        if (Convert.ToUInt32(obj1.Data[0]) == 175)
                        {
                            temp[65] = Convert.ToString(Convert.ToDouble((Convert.ToDouble(obj1.Data[2])) / 10));
                        }
                        if (Convert.ToUInt32(obj1.Data[0]) == 176)
                        {
                            temp[66] = Convert.ToString(Convert.ToDouble((Convert.ToDouble(obj1.Data[2])) / 10));
                        }
                        if (Convert.ToUInt32(obj1.Data[0]) == 177)
                        {
                            temp[67] = Convert.ToString(Convert.ToDouble((Convert.ToDouble(obj1.Data[2])) / 10));
                        }
                        if (Convert.ToUInt32(obj1.Data[0]) == 178)
                        {
                            temp[68] = Convert.ToString(Convert.ToDouble((Convert.ToDouble(obj1.Data[2])) / 10));
                        }
                        if (Convert.ToUInt32(obj1.Data[0]) == 179)
                        {
                            temp[69] = Convert.ToString(Convert.ToDouble((Convert.ToDouble(obj1.Data[2])) / 10));
                        }
                        if (Convert.ToUInt32(obj1.Data[0]) == 180)
                        {
                            temp[70] = Convert.ToString(Convert.ToDouble((Convert.ToDouble(obj1.Data[2])) / 10));
                        }
                        if (Convert.ToUInt32(obj1.Data[0]) == 181)
                        {
                            temp[71] = Convert.ToString(Convert.ToDouble((Convert.ToDouble(obj1.Data[2])) / 10));
                        }
                        if (Convert.ToUInt32(obj1.Data[0]) == 182)
                        {
                            temp[72] = Convert.ToString(Convert.ToDouble((Convert.ToDouble(obj1.Data[2])) / 10));
                        }
                        if (Convert.ToUInt32(obj1.Data[0]) == 183)
                        {
                            temp[73] = Convert.ToString(Convert.ToDouble((Convert.ToDouble(obj1.Data[2])) / 10));
                        }
                        if (Convert.ToUInt32(obj1.Data[0]) == 184)
                        {
                            temp[74] = Convert.ToString(Convert.ToDouble((Convert.ToDouble(obj1.Data[2])) / 10));
                        }
                        if (Convert.ToUInt32(obj1.Data[0]) == 185)
                        {
                            temp[75] = Convert.ToString(Convert.ToDouble((Convert.ToDouble(obj1.Data[2])) / 10));
                        }
                        if (Convert.ToUInt32(obj1.Data[0]) == 186)
                        {
                            temp[76] = Convert.ToString(Convert.ToDouble((Convert.ToDouble(obj1.Data[2])) / 10));
                        }
                        if (Convert.ToUInt32(obj1.Data[0]) == 187)
                        {
                            temp[77] = Convert.ToString(Convert.ToDouble((Convert.ToDouble(obj1.Data[2])) / 10));
                        }
                        if (Convert.ToUInt32(obj1.Data[0]) == 188)
                        {
                            temp[78] = Convert.ToString(Convert.ToDouble((Convert.ToDouble(obj1.Data[2])) / 10));
                        }
                        if (Convert.ToUInt32(obj1.Data[0]) == 189)
                        {
                            temp[79] = Convert.ToString(Convert.ToDouble((Convert.ToDouble(obj1.Data[2])) / 10));
                        }
                        if (Convert.ToUInt32(obj1.Data[0]) == 190)
                        {
                            temp[80] = Convert.ToString(Convert.ToDouble((Convert.ToDouble(obj1.Data[2])) / 10));
                        }
                        if (Convert.ToUInt32(obj1.Data[0]) == 191)
                        {
                            temp[81] = Convert.ToString(Convert.ToDouble((Convert.ToDouble(obj1.Data[2])) / 10));
                        }
                        if (Convert.ToUInt32(obj1.Data[0]) == 192)
                        {
                            temp[82] = Convert.ToString(Convert.ToDouble((Convert.ToDouble(obj1.Data[2])) / 10));
                        }
                        if (Convert.ToUInt32(obj1.Data[0]) == 193)
                        {
                            temp[83] = Convert.ToString(Convert.ToDouble((Convert.ToDouble(obj1.Data[2])) / 10));
                        }
                        if (Convert.ToUInt32(obj1.Data[0]) == 194)
                        {
                            temp[84] = Convert.ToString(Convert.ToDouble((Convert.ToDouble(obj1.Data[2])) / 10));
                        }
                        if (Convert.ToUInt32(obj1.Data[0]) == 195)
                        {
                            temp[85] = Convert.ToString(Convert.ToDouble((Convert.ToDouble(obj1.Data[2])) / 10));
                        }
                        if (Convert.ToUInt32(obj1.Data[0]) == 196)
                        {
                            temp[86] = Convert.ToString(Convert.ToDouble((Convert.ToDouble(obj1.Data[2])) / 10));
                        }
                        if (Convert.ToUInt32(obj1.Data[0]) == 197)
                        {
                            temp[87] = Convert.ToString(Convert.ToDouble((Convert.ToDouble(obj1.Data[2])) / 10));
                        }
                        if (Convert.ToUInt32(obj1.Data[0]) == 198)
                        {
                            temp[88] = Convert.ToString(Convert.ToDouble((Convert.ToDouble(obj1.Data[2])) / 10));
                        }
                        if (Convert.ToUInt32(obj1.Data[0]) == 199)
                        {
                            temp[89] = Convert.ToString(Convert.ToDouble((Convert.ToDouble(obj1.Data[2])) / 10));
                        }
                        if (Convert.ToUInt32(obj1.Data[0]) == 200)
                        {
                            temp[90] = Convert.ToString(Convert.ToDouble((Convert.ToDouble(obj1.Data[2])) / 10));
                        }
                        if (Convert.ToUInt32(obj1.Data[0]) == 201)
                        {
                            temp[91] = Convert.ToString(Convert.ToDouble((Convert.ToDouble(obj1.Data[2])) / 10));
                        }
                        if (Convert.ToUInt32(obj1.Data[0]) == 202)
                        {
                            temp[92] = Convert.ToString(Convert.ToDouble((Convert.ToDouble(obj1.Data[2])) / 10));
                        }
                        if (Convert.ToUInt32(obj1.Data[0]) == 203)
                        {
                            temp[93] = Convert.ToString(Convert.ToDouble((Convert.ToDouble(obj1.Data[2])) / 10));
                        }
                        if (Convert.ToUInt32(obj1.Data[0]) == 204)
                        {
                            temp[94] = Convert.ToString(Convert.ToDouble((Convert.ToDouble(obj1.Data[2])) / 10));
                        }
                        break;

                }

            }
            VCI_CAN_OBJ sendobj = new VCI_CAN_OBJ();
            if (listBox[0].Checked)
            {
                sendobj.ExternFlag = 1;
                sendobj.ID = System.Convert.ToUInt32("18ABFFFE", 16);
                sendobj.DataLen = 8;
                sendobj.Data[0] = Convert.ToByte(batvindex);
                sendobj.Data[1] = Convert.ToByte(batv % 256);
                sendobj.Data[2] = Convert.ToByte(batv / 256);
                sendobj.Data[3] = Convert.ToByte(battindex);
                sendobj.Data[4] = Convert.ToByte((batt) + 40);
                sendobj.Data[5] = Convert.ToByte(bataindex);
                sendobj.Data[6] = Convert.ToByte(bata % 256);
                sendobj.Data[7] = Convert.ToByte(bata / 256);
                if (VCI_Transmit(m_devtype, m_devind, m_canind, ref sendobj, 1) == 0)
                {
                    MessageBox.Show("发送失败", "错误",
                            MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                }
            }
            intimer = false;
            Marshal.FreeHGlobal(pt);
        }

        private void tileControl1_SelectedItemChanged(object sender, DevExpress.XtraEditors.TileItemEventArgs e)
        {
            navigationFrame1.SelectedPageIndex = tileGroup2.Items.IndexOf(e.Item);
        }

        unsafe public void display()
        {

            while (true)

            {
                dataGridView5.Rows[0].Cells[1].Value = Convert.ToSingle(v1[0]) / 1000;
                dataGridView5.Rows[1].Cells[1].Value = Convert.ToSingle(v1[1]) / 1000;
                dataGridView5.Rows[2].Cells[1].Value = Convert.ToSingle(v1[2]) / 1000;
                dataGridView5.Rows[3].Cells[1].Value = Convert.ToSingle(v1[3]) / 1000;
                dataGridView5.Rows[4].Cells[1].Value = Convert.ToSingle(v1[4]) / 1000;
                dataGridView5.Rows[5].Cells[1].Value = Convert.ToSingle(v1[5]) / 1000;
                dataGridView5.Rows[6].Cells[1].Value = Convert.ToSingle(v1[6]) / 1000;
                //dataGridView5.Rows[7].Cells[0].Value = "单体8电压";
                dataGridView5.Rows[7].Cells[1].Value = Convert.ToSingle(v1[7]) / 1000;
                dataGridView5.Rows[8].Cells[1].Value = Convert.ToSingle(v1[8]) / 1000;
                dataGridView5.Rows[9].Cells[1].Value = Convert.ToSingle(v1[9]) / 1000;
                dataGridView5.Rows[10].Cells[1].Value = Convert.ToSingle(v1[10]) / 1000;
                dataGridView5.Rows[11].Cells[1].Value = Convert.ToSingle(v1[11]) / 1000;
                dataGridView5.Rows[12].Cells[1].Value = Convert.ToSingle(v1[12]) / 1000;
                dataGridView5.Rows[13].Cells[1].Value = Convert.ToSingle(v1[13]) / 1000;
                dataGridView5.Rows[14].Cells[1].Value = Convert.ToSingle(v1[14]) / 1000;
                dataGridView5.Rows[15].Cells[1].Value = t1[0];
                dataGridView5.Rows[16].Cells[1].Value = t1[1];
                dataGridView5.Rows[17].Cells[1].Value = t1[2];

                dataGridView1.Rows[0].Cells[1].Value = Pcs变量.额定电压;
                dataGridView1.Rows[1].Cells[1].Value = Pcs变量.额定容量;
                //dataGridView1.Rows[2].Cells[0].Value = "从空节点数量";
                dataGridView1.Rows[2].Cells[1].Value = Pcs变量.电池类型;
                dataGridView1.Rows[3].Cells[1].Value = Pcs变量.电流传感器类型;
                dataGridView1.Rows[4].Cells[1].Value = Pcs变量.温度传感器类型;
                dataGridView1.Rows[5].Cells[1].Value = Pcs变量.单体最低电压;
                //dataGridView1.Rows[7].Cells[0].Value = "单体最低电压所在Pack号";
                dataGridView1.Rows[6].Cells[1].Value = Pcs变量.单体最低电压所在Pack内编号;
                dataGridView1.Rows[7].Cells[1].Value = Pcs变量.单体最高电压;
                //dataGridView1.Rows[10].Cells[0].Value = "单体最高电压所在Pack号";
                dataGridView1.Rows[8].Cells[1].Value = Pcs变量.单体最高电压所在Pack内编号;
                dataGridView1.Rows[9].Cells[1].Value = Convert.ToSingle(AveVoltage)/1000;
                dataGridView1.Rows[10].Cells[1].Value = Pcs变量.最低温度; 
                //dataGridView1.Rows[13].Cells[0].Value = "最低温度所在Pack号";
                dataGridView1.Rows[11].Cells[1].Value = Pcs变量.最低温度所在Pack内编号;
                dataGridView1.Rows[12].Cells[1].Value = Pcs变量.最高温度;
                //dataGridView1.Rows[16].Cells[0].Value = "最高温度所在Pack号";
                dataGridView1.Rows[13].Cells[1].Value = Pcs变量.最高温度所在Pack内编号;
                dataGridView1.Rows[14].Cells[1].Value = AveTemperture;
                dataGridView1.Rows[15].Cells[1].Value = Pcs变量.电压个数;
                dataGridView1.Rows[16].Cells[1].Value = Pcs变量.温度个数;
                dataGridView1.Rows[17].Cells[1].Value = Pcs变量.MOS温度;
                //dataGridView2.Rows[0].Cells[0].Value = "实际容量";
                dataGridView2.Rows[0].Cells[1].Value = Pcs变量.总电压;
                dataGridView2.Rows[1].Cells[1].Value = Pcs变量.实时电流;
                dataGridView2.Rows[2].Cells[1].Value = Pcs变量.SOC;
                dataGridView2.Rows[3].Cells[1].Value = Pcs变量.SOH;
                dataGridView2.Rows[4].Cells[1].Value = Pcs变量.BMS充电次数;
                dataGridView2.Rows[5].Cells[1].Value = Pcs变量.最大允许充电电压;
                dataGridView2.Rows[6].Cells[1].Value = Pcs变量.最小允许放电电压;
                dataGridView2.Rows[7].Cells[1].Value = Pcs变量.最大允许充电电流;
                dataGridView2.Rows[8].Cells[1].Value = Pcs变量.最大允许放电电流;
                dataGridView2.Rows[9].Cells[1].Value = Pcs变量.BMS最高告警故障状态;
                dataGridView2.Rows[10].Cells[1].Value = Pcs变量.BMU电压故障;
                dataGridView2.Rows[11].Cells[1].Value = Pcs变量.BMS工作状态;
                dataGridView2.Rows[12].Cells[1].Value = Pcs变量.温度传感器故障;
                //dataGridView2.Rows[13].Cells[0].Value = "烟雾传感器连接故障";
                dataGridView2.Rows[13].Cells[1].Value = Pcs变量.电流传感器故障;
                dataGridView2.Rows[14].Cells[1].Value = Pcs变量.绝缘模块连接故障;
                dataGridView2.Rows[15].Cells[1].Value = Pcs变量.放电接触器状态;
                dataGridView2.Rows[16].Cells[1].Value = Pcs变量.充电接触器状态;
                //dataGridView2.Rows[17].Cells[0].Value = "烟雾传感器故障";
                //dataGridView2.Rows[18].Cells[0].Value = "加热故障";
                dataGridView3.Rows[0].Cells[1].Value = Pcs变量.轻微漏电;
                dataGridView3.Rows[1].Cells[1].Value = Pcs变量.轻微单体过高;
                dataGridView3.Rows[2].Cells[1].Value = Pcs变量.轻微单体过低;
                dataGridView3.Rows[3].Cells[1].Value = Pcs变量.轻微总压过高;
                dataGridView3.Rows[4].Cells[1].Value = Pcs变量.轻微总压过低;
                dataGridView3.Rows[5].Cells[1].Value = Pcs变量.轻微MOS温度过高;
                dataGridView3.Rows[6].Cells[1].Value = Pcs变量.轻微充电电流过高;
                dataGridView3.Rows[7].Cells[1].Value = Pcs变量.轻微放电电流过高;
                dataGridView3.Rows[8].Cells[1].Value = Pcs变量.轻微充电温度过高;
                dataGridView3.Rows[9].Cells[1].Value = Pcs变量.轻微充电温度过低;
                dataGridView3.Rows[10].Cells[1].Value = Pcs变量.轻微放电温度过高;
                dataGridView3.Rows[11].Cells[1].Value = Pcs变量.轻微放电温度过低;
                dataGridView3.Rows[12].Cells[1].Value = Pcs变量.轻微单体压差过大;
                dataGridView3.Rows[13].Cells[1].Value = Pcs变量.轻微温差过大;
                dataGridView3.Rows[14].Cells[1].Value = Pcs变量.轻微SOC过低;
                dataGridView3.Rows[15].Cells[1].Value = Pcs变量.轻微SOC过高;
                dataGridView3.Rows[0].Cells[2].Value = Pcs变量.一般漏电;
                dataGridView3.Rows[1].Cells[2].Value = Pcs变量.一般单体过高;
                dataGridView3.Rows[2].Cells[2].Value = Pcs变量.一般单体过低;
                dataGridView3.Rows[3].Cells[2].Value = Pcs变量.一般总压过高;
                dataGridView3.Rows[4].Cells[2].Value = Pcs变量.一般总压过低;
                dataGridView3.Rows[5].Cells[2].Value = Pcs变量.一般MOS温度过高;
                dataGridView3.Rows[6].Cells[2].Value = Pcs变量.一般充电电流过高;
                dataGridView3.Rows[7].Cells[2].Value = Pcs变量.一般放电电流过高;
                dataGridView3.Rows[8].Cells[2].Value = Pcs变量.一般充电温度过高;
                dataGridView3.Rows[9].Cells[2].Value = Pcs变量.一般充电温度过低;
                dataGridView3.Rows[10].Cells[2].Value = Pcs变量.一般放电温度过高;
                dataGridView3.Rows[11].Cells[2].Value = Pcs变量.一般放电温度过低;
                dataGridView3.Rows[12].Cells[2].Value = Pcs变量.一般单体压差过大;
                dataGridView3.Rows[13].Cells[2].Value = Pcs变量.一般温差过大;
                dataGridView3.Rows[14].Cells[2].Value = Pcs变量.一般SOC过低;
                dataGridView3.Rows[15].Cells[2].Value = Pcs变量.一般SOC过高;
                dataGridView3.Rows[0].Cells[3].Value = Pcs变量.严重漏电;
                dataGridView3.Rows[1].Cells[3].Value = Pcs变量.严重单体过高;
                dataGridView3.Rows[2].Cells[3].Value = Pcs变量.严重单体过低;
                dataGridView3.Rows[3].Cells[3].Value = Pcs变量.严重总压过高;
                dataGridView3.Rows[4].Cells[3].Value = Pcs变量.严重总压过低;
                dataGridView3.Rows[5].Cells[3].Value = Pcs变量.严重MOS温度过高;
                dataGridView3.Rows[6].Cells[3].Value = Pcs变量.严重充电电流过高;
                dataGridView3.Rows[7].Cells[3].Value = Pcs变量.严重放电电流过高;
                dataGridView3.Rows[8].Cells[3].Value = Pcs变量.严重充电温度过高;
                dataGridView3.Rows[9].Cells[3].Value = Pcs变量.严重充电温度过低;
                dataGridView3.Rows[10].Cells[3].Value = Pcs变量.严重放电温度过高;
                dataGridView3.Rows[11].Cells[3].Value = Pcs变量.严重放电温度过低;
                dataGridView3.Rows[12].Cells[3].Value = Pcs变量.严重单体压差过大;
                dataGridView3.Rows[13].Cells[3].Value = Pcs变量.严重温差过大;
                dataGridView3.Rows[14].Cells[3].Value = Pcs变量.严重SOC过低;
                dataGridView3.Rows[15].Cells[3].Value = Pcs变量.严重SOC过高;
               
                dataGridView6.Rows[0].Cells[1].Value = Convert.ToSingle(AveVoltage) / 1000;
                dataGridView6.Rows[1].Cells[1].Value = Pcs变量.单体最高电压;
                dataGridView6.Rows[2].Cells[1].Value = Pcs变量.单体最低电压;
                dataGridView6.Rows[3].Cells[1].Value = Pcs变量.单体最高电压所在Pack内编号;
                dataGridView6.Rows[4].Cells[1].Value = Pcs变量.单体最低电压所在Pack内编号;
                dataGridView6.Rows[5].Cells[1].Value = AveTemperture;
                dataGridView6.Rows[6].Cells[1].Value = Pcs变量.最高温度;
                dataGridView6.Rows[7].Cells[1].Value = Pcs变量.最低温度;
                dataGridView6.Rows[8].Cells[1].Value = Pcs变量.最高温度所在Pack内编号;
                dataGridView6.Rows[9].Cells[1].Value = Pcs变量.最低温度所在Pack内编号;
                dataGridView6.Rows[10].Cells[1].Value = Pcs变量.电池序列号;
                dataGridView6.Rows[11].Cells[1].Value = Pcs变量.MOS温度;

                Thread.Sleep(50);
                //Task.WaitAll();
            }
        }

        unsafe public void display1()
        {
            while (报文 == true)
            {
                UInt32 res = new UInt32();
                res = VCI_GetReceiveNum(m_devtype, m_devind, m_canind);
                //if (res == 0)
                //    return;
                UInt32 con_maxlen = 50;
                IntPtr pt = Marshal.AllocHGlobal(Marshal.SizeOf(typeof(VCI_CAN_OBJ)) * (Int32)con_maxlen);
                res = VCI_Receive(m_devtype, m_devind, m_canind, pt, con_maxlen, 100);
                for (int i = 0; i < res; i++)
                {
                    VCI_CAN_OBJ obj1 = (VCI_CAN_OBJ)Marshal.PtrToStructure((IntPtr)((UInt32)pt + i * Marshal.SizeOf(typeof(VCI_CAN_OBJ))), typeof(VCI_CAN_OBJ));
                    string RecTime = DateTime.Now.ToString("HH:mm:ss");
                    canstr = RecTime;
                    canstr += "   接收到数据:   ";
                    canstr += "    帧ID:    0x" + System.Convert.ToString((Int32)obj1.ID, 16);
                    if (obj1.RemoteFlag == 0)
                    {
                        canstr += "   数据:  ";
                        byte len = (byte)(obj1.DataLen % 9);
                        byte j = 0;
                        if (j++ < len)
                            canstr += "  " + string.Format("{0:X2}", obj1.Data[0]);
                        if (j++ < len)
                            canstr += "  " + string.Format("{0:X2}", obj1.Data[1]);
                        if (j++ < len)
                            canstr += "  " + string.Format("{0:X2}", obj1.Data[2]);
                        if (j++ < len)
                            canstr += "  " + string.Format("{0:X2}", obj1.Data[3]);
                        if (j++ < len)
                            canstr += "  " + string.Format("{0:X2}", obj1.Data[4]);
                        if (j++ < len)
                            canstr += "  " + string.Format("{0:X2}", obj1.Data[5]);
                        if (j++ < len)
                            canstr += "  " + string.Format("{0:X2}", obj1.Data[6]);
                        if (j++ < len)
                            canstr += "  " + string.Format("{0:X2}", obj1.Data[7]);
                    }
                    if (运行 == true && (Pcs变量.BMS最高告警故障状态 == "无告警")) { animation(Properties.Resources.bms运行); }
                    if (运行 == false) { animation(Properties.Resources.bms); }
                    if (运行 == true && (Pcs变量.BMS最高告警故障状态 != "无告警")) { animation(Properties.Resources.bms故障); }
                    list();
                    canlist();
                    Thread.Sleep(250);
                }
                Marshal.FreeHGlobal(pt);
                //Task.WaitAll();
            }

        }
        private void animation(Image imag)
        {
            if (bmspic.InvokeRequired)
            {
                while (!bmspic.IsHandleCreated)
                {
                    if (bmspic.Disposing || bmspic.IsDisposed)
                        return;
                }
                changepic a = new changepic(changeanimation);
                bmspic.BeginInvoke(a, new object[] { imag });
            }
        }
        private void list()
        {
            if (BMS_list.InvokeRequired)
            {
                while (!BMS_list.IsHandleCreated)
                {
                    if (BMS_list.Disposing || BMS_list.IsDisposed)
                        return;
                }
                changelist b = new changelist(changelistvalue);
                BMS_list.BeginInvoke(b, new object[] { });
            }
        }
        private void changeanimation(Image imag)
        {
            bmspic.Image = imag;
        }
        private void changelistvalue()
        {
            if (运行 == true) { BMS_list.Visible = true; }
            if (运行 == false) { BMS_list.Visible = false; }
            BMS_list.Items[0] = ("SOC：" + Convert.ToDouble(Pcs变量.SOC) + "%"); BMS_list.Items[1] = ("总电压：" + Convert.ToDouble(Pcs变量.总电压) + "V"); BMS_list.Items[2] = ("总电流：" + Convert.ToDouble(Pcs变量.实时电流) + "A");
            BMS_list.Items[3] = ("平均电压：" + Convert.ToDouble(AveVoltage) * 0.001 + "V"); BMS_list.Items[4] = ("最大电压：" + Convert.ToDouble(Pcs变量.单体最高电压) + "V"); BMS_list.Items[5] = ("最小电压：" + Convert.ToDouble(Pcs变量.单体最低电压)  + "V");
            BMS_list.Items[6] = ("平均温度：" + Convert.ToDouble(AveTemperture) + "℃");
            BMS_list.Items[7] = ("最高温度：" + Convert.ToDouble(Pcs变量.最高温度) + "℃"); BMS_list.Items[8] = ("最低温度：" + Convert.ToDouble(Pcs变量.最低温度) + "℃"); BMS_list.Items[9] = ("容量：" + Convert.ToDouble(Pcs变量.额定容量) + "AH");
            BMS_list.Items[10] = ("SOH：" + Pcs变量.SOH); BMS_list.Items[11] = ("MOS温度：" + Pcs变量.MOS温度 + "℃"); BMS_list.Items[12] = ("告警：" + Pcs变量.BMS最高告警故障状态);
        }

        private void 存储pic_Click(object sender, EventArgs e)
        {
            if (savedata == false)
            {
                存储pic.Image = Properties.Resources.存储1;
                Thread SaveThread = new Thread(new ThreadStart(savetoexcel));
                SaveThread.IsBackground = true;
                System.IO.Directory.CreateDirectory(System.Environment.CurrentDirectory + "\\MonitorData");         //创建文件夹
                fileName = (System.Environment.CurrentDirectory + "\\MonitorData" + "\\" + RecStartTime + ".xlsx");  //文件的保存路径和文件名
                ExcelApp = new Microsoft.Office.Interop.Excel.Application();// 创建Excel文档类
                ExcelDoc = ExcelApp.Workbooks.Add(Type.Missing);//创建EXCEL文档
                xlSheet = ExcelDoc.Worksheets.Add(Type.Missing, Type.Missing, Type.Missing, Type.Missing);// 创建一个EXCEL页
                ExcelApp.DisplayAlerts = false;
                Row = 1;
                savedata = true;
                SaveThread.Start();
            }
            else
            {
                存储pic.Image = Properties.Resources.存储; savedata = false;
                ExcelDoc.Close(Type.Missing, fileName, Type.Missing);
                ExcelApp.Quit();
            }
        }
        private void savetoexcel()
        {
            while (savedata == true)
            {
                try
                {
                    RecRealTime = DateTime.Now.ToString("HH:mm:ss");
                    if (Row == 1)
                    {
                        xlSheet.Cells[Row, 1] = "记录时间"; xlSheet.Cells[Row, 2] = "SOC"; xlSheet.Cells[Row, 3] = "电流"; xlSheet.Cells[Row, 4] = "总电压";
                        xlSheet.Cells[Row, 5] = "平均电压"; xlSheet.Cells[Row, 6] = "最高电压"; xlSheet.Cells[Row, 7] = "最低电压";
                        xlSheet.Cells[Row, 8] = "电池1"; xlSheet.Cells[Row, 9] = "电池2";
                        xlSheet.Cells[Row, 10] = "电池3"; xlSheet.Cells[Row, 11] = "电池4"; xlSheet.Cells[Row, 12] = "电池5"; xlSheet.Cells[Row, 13] = "电池6";
                        xlSheet.Cells[Row, 14] = "电池7"; xlSheet.Cells[Row, 15] = "电池8"; xlSheet.Cells[Row, 16] = "电池9"; xlSheet.Cells[Row, 17] = "电池10";
                        xlSheet.Cells[Row, 18] = "电池11"; xlSheet.Cells[Row, 19] = "电池12"; xlSheet.Cells[Row, 20] = "电池13"; xlSheet.Cells[Row, 21] = "电池14";
                        xlSheet.Cells[Row, 22] = "电池15"; xlSheet.Cells[Row, 23] = "电池16"; xlSheet.Cells[Row, 24] = "温度1"; xlSheet.Cells[Row, 25] = "温度2";
                        xlSheet.Cells[Row, 26] = "温度3"; xlSheet.Cells[Row, 27] = "温度4"; xlSheet.Cells[Row, 28] = "温度5"; xlSheet.Cells[Row, 29] = "温度6";
                        xlSheet.Cells[Row, 30] = "温度7"; xlSheet.Cells[Row, 31] = "温度8";
                    }
                    else /*if (2 <= Row && Row < 65536)*/
                    {

                        xlSheet.Cells[Row, 1] = RecRealTime;
                        xlSheet.Cells[Row, 2] = Pcs变量.SOC;
                        xlSheet.Cells[Row, 3] = Pcs变量.实时电流;
                        xlSheet.Cells[Row, 4] = Pcs变量.总电压;
                        xlSheet.Cells[Row, 5] = Convert.ToDouble(AveVoltage) * 0.001;
                        xlSheet.Cells[Row, 6] = Pcs变量.单体最高电压;
                        xlSheet.Cells[Row, 7] = Pcs变量.单体最低电压;
                        for (int k = 0; k < 16; k++) { xlSheet.Cells[Row, k + 8] = Convert.ToSingle(v1[k])/1000; }
                        for (int k = 0; k < 8; k++) { xlSheet.Cells[Row, k + 24] = t1[k]; }
                    }

                    xlSheet.SaveAs(fileName);
                    Row++;
                    Thread.Sleep(1000);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.ToString(), "提示", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }

        }

        private void tileBar1_SelectedItemChanged(object sender, DevExpress.XtraEditors.TileItemEventArgs e)
        {
            navigationFrame2.SelectedPageIndex = tileBarGroup2.Items.IndexOf(e.Item);
        }
        private static string DecimalToBinary1(uint D)
        {
            List<uint> vYuShu = new List<uint>();
            uint vTempValue = D;
            for (; ; )
            {
                uint tempYS = vTempValue % 2;
                vYuShu.Add(tempYS);

                vTempValue = vTempValue / 2;
                if (vTempValue == 0)
                    break;
            }
            string strBinary = "";
            for (int i = vYuShu.Count - 1; i >= 0; i--)
            {
                strBinary += vYuShu[i];
            }
            return strBinary;
        }
        private void simpleButton1_Click(object sender, EventArgs e)
        {
            if (密码.Text == "0123456789")
            {
                tileBarItem1.Enabled = true; tileBarItem2.Enabled = true; tileBarItem3.Enabled = true; tileBarItem4.Enabled = true;
                navigationFrame2.Visible = true; 密码.Text = "";
            }
            else
            {
                MessageBox.Show("密码错误", "错误",
                       MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
        }
        #region 模拟
        private void 设置参数_Click(object sender, EventArgs e)
        {
            单体电压值.Enabled = true; 温度值.Enabled = true; 电流值.Enabled = true; 保存.Enabled = true;
            电池号.Enabled = true; 温度号.Enabled = true; 电流.Enabled = true;
        }

        private void 保存_Click(object sender, EventArgs e)
        {
            batv = Convert.ToUInt32(Convert.ToSingle(单体电压值.Text) * 1000);
            batt = Convert.ToInt32(温度值.Text);
            bata = Convert.ToInt32(Convert.ToSingle(电流值.Text) * 10)  + 32000;
            batvindex = 电池号.SelectedIndex;
            battindex = 温度号.SelectedIndex;
            bataindex = 电流.SelectedIndex;
            单体电压值.Enabled = false; 温度值.Enabled = false; 电流值.Enabled = false; 保存.Enabled = false;
            电池号.Enabled = false; 温度号.Enabled = false; 电流.Enabled = false;开始模拟.Enabled = true;
        }
 
        private void 单体电压值_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!(Char.IsNumber(e.KeyChar)) && e.KeyChar != (char)13 && e.KeyChar != (char)8 && e.KeyChar != (char)'.')
            {
                e.Handled = true;
            }
        }

        private void 温度值_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!(Char.IsNumber(e.KeyChar)) && e.KeyChar != (char)13 && e.KeyChar != (char)8  && e.KeyChar != (char)'-')
            {
                e.Handled = true;
            }
        }

        private void 电流值_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!(Char.IsNumber(e.KeyChar)) && e.KeyChar != (char)13 && e.KeyChar != (char)8 && e.KeyChar != (char)'-')
            {
                e.Handled = true;
            }
        }

        private void 开始模拟_Click(object sender, EventArgs e)
        {
            try
            {
                if (Convert.ToSingle(单体电压值.Text) > 5 || Convert.ToSingle(单体电压值.Text) < 0)
                {
                    MessageBox.Show("请输入0-5伏之间的电压", "提示", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
                if (Convert.ToSingle(温度值.Text) > 120 || Convert.ToSingle(温度值.Text) < -40)
                {
                    MessageBox.Show("请输入-40—120℃之间的温度", "提示", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

            }
            catch
            {
                MessageBox.Show("电压、温度请输入完整", "提示", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        #endregion
        #region 控制输入
        private void 容量_KeyPress(object sender, KeyPressEventArgs e)
        {
            if ((e.KeyChar < 48 || e.KeyChar > 57) && e.KeyChar != 8 && e.KeyChar != 13 && e.KeyChar != 45 && e.KeyChar != 46)
                e.Handled = true;

            if (e.KeyChar != '\b' && (((System.Windows.Forms.TextBox)sender).SelectionStart) > (((System.Windows.Forms.TextBox)sender).Text.LastIndexOf('.')) + 1 && ((System.Windows.Forms.TextBox)sender).Text.IndexOf(".") >= 0)
                e.Handled = true;                    
        }
        private void 轻微漏电时间1_KeyPress(object sender, KeyPressEventArgs e)
        {
            if ((e.KeyChar < 48 || e.KeyChar > 57) && e.KeyChar != 8 && e.KeyChar != 13 && e.KeyChar != 45 && e.KeyChar != 46)
                e.Handled = true;

            if (e.KeyChar != '\b' && (((System.Windows.Forms.TextBox)sender).SelectionStart) > (((System.Windows.Forms.TextBox)sender).Text.LastIndexOf('.')) + 1 && ((System.Windows.Forms.TextBox)sender).Text.IndexOf(".") >= 0)
                e.Handled = true;
        }

        private void 轻微漏电时间2_KeyPress(object sender, KeyPressEventArgs e)
        {
            if ((e.KeyChar < 48 || e.KeyChar > 57) && e.KeyChar != 8 && e.KeyChar != 13 && e.KeyChar != 45 && e.KeyChar != 46)
                e.Handled = true;

            if (e.KeyChar != '\b' && (((System.Windows.Forms.TextBox)sender).SelectionStart) > (((System.Windows.Forms.TextBox)sender).Text.LastIndexOf('.')) + 1 && ((System.Windows.Forms.TextBox)sender).Text.IndexOf(".") >= 0)
                e.Handled = true;

        }

        private void 轻微漏电时间3_KeyPress(object sender, KeyPressEventArgs e)
        {
            if ((e.KeyChar < 48 || e.KeyChar > 57) && e.KeyChar != 8 && e.KeyChar != 13 && e.KeyChar != 45 && e.KeyChar != 46)
                e.Handled = true;

            if (e.KeyChar != '\b' && (((System.Windows.Forms.TextBox)sender).SelectionStart) > (((System.Windows.Forms.TextBox)sender).Text.LastIndexOf('.')) + 1 && ((System.Windows.Forms.TextBox)sender).Text.IndexOf(".") >= 0)
                e.Handled = true;

        }

        private void 总压过高时间1_KeyPress(object sender, KeyPressEventArgs e)
        {
            if ((e.KeyChar < 48 || e.KeyChar > 57) && e.KeyChar != 8 && e.KeyChar != 13 && e.KeyChar != 45 && e.KeyChar != 46)
                e.Handled = true;

            if (e.KeyChar != '\b' && (((System.Windows.Forms.TextBox)sender).SelectionStart) > (((System.Windows.Forms.TextBox)sender).Text.LastIndexOf('.')) + 1 && ((System.Windows.Forms.TextBox)sender).Text.IndexOf(".") >= 0)
                e.Handled = true;
        }

        private void 总压过高时间2_KeyPress(object sender, KeyPressEventArgs e)
        {
            if ((e.KeyChar < 48 || e.KeyChar > 57) && e.KeyChar != 8 && e.KeyChar != 13 && e.KeyChar != 45 && e.KeyChar != 46)
                e.Handled = true;

            if (e.KeyChar != '\b' && (((System.Windows.Forms.TextBox)sender).SelectionStart) > (((System.Windows.Forms.TextBox)sender).Text.LastIndexOf('.')) + 1 && ((System.Windows.Forms.TextBox)sender).Text.IndexOf(".") >= 0)
                e.Handled = true;
        }

        private void 总压过高时间3_KeyPress(object sender, KeyPressEventArgs e)
        {
            if ((e.KeyChar < 48 || e.KeyChar > 57) && e.KeyChar != 8 && e.KeyChar != 13 && e.KeyChar != 45 && e.KeyChar != 46)
                e.Handled = true;

            if (e.KeyChar != '\b' && (((System.Windows.Forms.TextBox)sender).SelectionStart) > (((System.Windows.Forms.TextBox)sender).Text.LastIndexOf('.')) + 1 && ((System.Windows.Forms.TextBox)sender).Text.IndexOf(".") >= 0)
                e.Handled = true;

        }
        private void 充电温度过低时间1_KeyPress(object sender, KeyPressEventArgs e)
        {
            if ((e.KeyChar < 48 || e.KeyChar > 57) && e.KeyChar != 8 && e.KeyChar != 13 && e.KeyChar != 45 && e.KeyChar != 46)
                e.Handled = true;

            if (e.KeyChar != '\b' && (((System.Windows.Forms.TextBox)sender).SelectionStart) > (((System.Windows.Forms.TextBox)sender).Text.LastIndexOf('.')) + 1 && ((System.Windows.Forms.TextBox)sender).Text.IndexOf(".") >= 0)
                e.Handled = true;

        }

        private void 充电温度过低时间2_KeyPress(object sender, KeyPressEventArgs e)
        {
            if ((e.KeyChar < 48 || e.KeyChar > 57) && e.KeyChar != 8 && e.KeyChar != 13 && e.KeyChar != 45 && e.KeyChar != 46)
                e.Handled = true;

            if (e.KeyChar != '\b' && (((System.Windows.Forms.TextBox)sender).SelectionStart) > (((System.Windows.Forms.TextBox)sender).Text.LastIndexOf('.')) + 1 && ((System.Windows.Forms.TextBox)sender).Text.IndexOf(".") >= 0)
                e.Handled = true;

        }

        private void 充电温度过低时间3_KeyPress(object sender, KeyPressEventArgs e)
        {
            if ((e.KeyChar < 48 || e.KeyChar > 57) && e.KeyChar != 8 && e.KeyChar != 13 && e.KeyChar != 45 && e.KeyChar != 46)
                e.Handled = true;

            if (e.KeyChar != '\b' && (((System.Windows.Forms.TextBox)sender).SelectionStart) > (((System.Windows.Forms.TextBox)sender).Text.LastIndexOf('.')) + 1 && ((System.Windows.Forms.TextBox)sender).Text.IndexOf(".") >= 0)
                e.Handled = true;
        }
        #endregion

        #region 标定
        unsafe private void SOC标定_Click(object sender, EventArgs e)
        {
            SOC标定.Enabled = false;
            if (m_bOpen == 0)
                return;
            VCI_CAN_OBJ sendobj = new VCI_CAN_OBJ();
            sendobj.ExternFlag = 1;
            sendobj.ID = System.Convert.ToUInt32("18000029", 16);
            sendobj.DataLen = 8;
            sendobj.Data[0] = Convert.ToByte("127");
            sendobj.Data[1] = Convert.ToByte("135");
            sendobj.Data[2] = Convert.ToByte("00");
            sendobj.Data[3] = Convert.ToByte(Convert.ToDouble(SOC.Text) * 2.5);
            sendobj.Data[4] = Convert.ToByte("00");
            sendobj.Data[5] = Convert.ToByte("00");
            sendobj.Data[6] = Convert.ToByte("00");
            sendobj.Data[7] = Convert.ToByte("00");
            if (VCI_Transmit(m_devtype, m_devind, m_canind, ref sendobj, 1) == 0)
            {
                MessageBox.Show("发送失败", "错误",
                        MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
            System.Threading.Thread.Sleep(100);
            VCI_CAN_OBJ sendobj1 = new VCI_CAN_OBJ();
            sendobj1.ExternFlag = 1;
            sendobj1.ID = System.Convert.ToUInt32("18000029", 16);
            sendobj1.DataLen = 8;
            sendobj1.Data[0] = Convert.ToByte("191");
            sendobj1.Data[1] = Convert.ToByte("135");
            sendobj1.Data[2] = Convert.ToByte("00");
            sendobj1.Data[3] = Convert.ToByte("00");
            sendobj1.Data[4] = Convert.ToByte("00");
            sendobj1.Data[5] = Convert.ToByte("00");
            sendobj1.Data[6] = Convert.ToByte("00");
            sendobj1.Data[7] = Convert.ToByte("00");
            if (VCI_Transmit(m_devtype, m_devind, m_canind, ref sendobj1, 1) == 0)
            {
                MessageBox.Show("发送失败", "错误",
                        MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
            System.Threading.Thread.Sleep(1000);
            callback1.ID1 = temp[0];
            SOC标定.Enabled = true;
        }
       unsafe private void SOH标定_Click(object sender, EventArgs e)
        {
            SOH标定.Enabled = false;
            if (m_bOpen == 0)
                return;
            VCI_CAN_OBJ sendobj = new VCI_CAN_OBJ();
            sendobj.ExternFlag = 1;
            sendobj.ID = System.Convert.ToUInt32("18000029", 16);
            sendobj.DataLen = 8;
            sendobj.Data[0] = Convert.ToByte("127");
            sendobj.Data[1] = Convert.ToByte("191");
            sendobj.Data[2] = Convert.ToByte("00");
            sendobj.Data[3] = Convert.ToByte(Convert.ToDouble(SOH.Text) * 2.5);
            sendobj.Data[4] = Convert.ToByte("00");
            sendobj.Data[5] = Convert.ToByte("00");
            sendobj.Data[6] = Convert.ToByte("00");
            sendobj.Data[7] = Convert.ToByte("00");
            if (VCI_Transmit(m_devtype, m_devind, m_canind, ref sendobj, 1) == 0)
            {
                MessageBox.Show("发送失败", "错误",
                        MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
            System.Threading.Thread.Sleep(100);
            VCI_CAN_OBJ sendobj1 = new VCI_CAN_OBJ();
            sendobj1.ExternFlag = 1;
            sendobj1.ID = System.Convert.ToUInt32("18000029", 16);
            sendobj1.DataLen = 8;
            sendobj1.Data[0] = Convert.ToByte("191");
            sendobj1.Data[1] = Convert.ToByte("191");
            sendobj1.Data[2] = Convert.ToByte("00");
            sendobj1.Data[3] = Convert.ToByte("00");
            sendobj1.Data[4] = Convert.ToByte("00");
            sendobj1.Data[5] = Convert.ToByte("00");
            sendobj1.Data[6] = Convert.ToByte("00");
            sendobj1.Data[7] = Convert.ToByte("00");
            if (VCI_Transmit(m_devtype, m_devind, m_canind, ref sendobj1, 1) == 0)
            {
                MessageBox.Show("发送失败", "错误",
                        MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
            System.Threading.Thread.Sleep(1000);
            callback1.ID2 = temp[1];
            SOH标定.Enabled = true;
        }
       unsafe private void 容量标定_Click(object sender, EventArgs e)
        {
            容量标定.Enabled = false;
            UInt32[] a = new UInt32[4];
            if (m_bOpen == 0)
                return;
            VCI_CAN_OBJ sendobj = new VCI_CAN_OBJ();
            sendobj.ExternFlag = 1;
            sendobj.ID = System.Convert.ToUInt32("18000029", 16);
            sendobj.DataLen = 8;
            sendobj.Data[0] = Convert.ToByte("127");
            sendobj.Data[1] = Convert.ToByte("136");
            sendobj.Data[2] = Convert.ToByte("00");
            sendobj.Data[3] = Convert.ToByte((Convert.ToDouble(容量.Text) * 10) % 256);
            sendobj.Data[4] = Convert.ToByte("00");
            sendobj.Data[5] = Convert.ToByte("00");
            sendobj.Data[6] = Convert.ToByte("00");
            sendobj.Data[7] = Convert.ToByte("00");
            if (VCI_Transmit(m_devtype, m_devind, m_canind, ref sendobj, 1) == 0)
            {
                MessageBox.Show("发送失败", "错误",
                        MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
            System.Threading.Thread.Sleep(100);

            VCI_CAN_OBJ sendobj1 = new VCI_CAN_OBJ();
            sendobj1.ExternFlag = 1;
            sendobj1.ID = System.Convert.ToUInt32("18000029", 16);
            sendobj1.DataLen = 8;
            sendobj1.Data[0] = Convert.ToByte("127");
            sendobj1.Data[1] = Convert.ToByte("137");
            sendobj1.Data[2] = Convert.ToByte("00");
            sendobj1.Data[3] = Convert.ToByte(((Convert.ToDouble(容量.Text) *10) / 256));
            sendobj1.Data[4] = Convert.ToByte("00");
            sendobj1.Data[5] = Convert.ToByte("00");
            sendobj1.Data[6] = Convert.ToByte("00");
            sendobj1.Data[7] = Convert.ToByte("00");
            if (VCI_Transmit(m_devtype, m_devind, m_canind, ref sendobj1, 1) == 0)
            {
                MessageBox.Show("发送失败", "错误",
                        MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
            System.Threading.Thread.Sleep(100);

            //VCI_CAN_OBJ sendobj2 = new VCI_CAN_OBJ();
            //sendobj2.ExternFlag = 1;
            //sendobj2.ID = System.Convert.ToUInt32("18000029", 16);
            //sendobj2.DataLen = 8;
            //sendobj2.Data[0] = Convert.ToByte("127");
            //sendobj2.Data[1] = Convert.ToByte("138");
            //sendobj2.Data[2] = Convert.ToByte("00");
            //sendobj2.Data[3] = Convert.ToByte((((Convert.ToUInt32(容量.Text) * 36000) / 256) / 256) % 256);
            //sendobj2.Data[4] = Convert.ToByte("00");
            //sendobj2.Data[5] = Convert.ToByte("00");
            //sendobj2.Data[6] = Convert.ToByte("00");
            //sendobj2.Data[7] = Convert.ToByte("00");
            //if (VCI_Transmit(m_devtype, m_devind, m_canind, ref sendobj2, 1) == 0)
            //{
            //    MessageBox.Show("发送失败", "错误",
            //            MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            //}
            //System.Threading.Thread.Sleep(500);

            //VCI_CAN_OBJ sendobj3 = new VCI_CAN_OBJ();
            //sendobj3.ExternFlag = 1;
            //sendobj3.ID = System.Convert.ToUInt32("18000029", 16);
            //sendobj3.DataLen = 8;
            //sendobj3.Data[0] = Convert.ToByte("127");
            //sendobj3.Data[1] = Convert.ToByte("139");
            //sendobj3.Data[2] = Convert.ToByte("00");
            //sendobj3.Data[3] = Convert.ToByte((((Convert.ToUInt32(容量.Text) * 36000) / 256) / 256) / 256);
            //sendobj3.Data[4] = Convert.ToByte("00");
            //sendobj3.Data[5] = Convert.ToByte("00");
            //sendobj3.Data[6] = Convert.ToByte("00");
            //sendobj3.Data[7] = Convert.ToByte("00");
            //if (VCI_Transmit(m_devtype, m_devind, m_canind, ref sendobj3, 1) == 0)
            //{
            //    MessageBox.Show("发送失败", "错误",
            //            MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            //}
            //System.Threading.Thread.Sleep(500);

            VCI_CAN_OBJ sendobj4 = new VCI_CAN_OBJ();
            sendobj4.ExternFlag = 1;
            sendobj4.ID = System.Convert.ToUInt32("18000029", 16);
            sendobj4.DataLen = 8;
            sendobj4.Data[0] = Convert.ToByte("191");
            sendobj4.Data[1] = Convert.ToByte("136");
            sendobj4.Data[2] = Convert.ToByte("00");
            sendobj4.Data[3] = Convert.ToByte("00");
            sendobj4.Data[4] = Convert.ToByte("00");
            sendobj4.Data[5] = Convert.ToByte("00");
            sendobj4.Data[6] = Convert.ToByte("00");
            sendobj4.Data[7] = Convert.ToByte("00");
            if (VCI_Transmit(m_devtype, m_devind, m_canind, ref sendobj4, 1) == 0)
            {
                MessageBox.Show("发送失败", "错误",
                        MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
            System.Threading.Thread.Sleep(1000);
            VCI_CAN_OBJ sendobj6 = new VCI_CAN_OBJ();
            sendobj6.ExternFlag = 1;
            sendobj6.ID = System.Convert.ToUInt32("18000029", 16);
            sendobj6.DataLen = 8;
            sendobj6.Data[0] = Convert.ToByte("191");
            sendobj6.Data[1] = Convert.ToByte("137");
            sendobj6.Data[2] = Convert.ToByte("00");
            sendobj6.Data[3] = Convert.ToByte("00");
            sendobj6.Data[4] = Convert.ToByte("00");
            sendobj6.Data[5] = Convert.ToByte("00");
            sendobj6.Data[6] = Convert.ToByte("00");
            sendobj6.Data[7] = Convert.ToByte("00");
            if (VCI_Transmit(m_devtype, m_devind, m_canind, ref sendobj6, 1) == 0)
            {
                MessageBox.Show("发送失败", "错误",
                        MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
            System.Threading.Thread.Sleep(1500);
            //VCI_CAN_OBJ sendobj8 = new VCI_CAN_OBJ();
            //sendobj8.ExternFlag = 1;
            //sendobj8.ID = System.Convert.ToUInt32("18000029", 16);
            //sendobj8.DataLen = 8;
            //sendobj8.Data[0] = Convert.ToByte("191");
            //sendobj8.Data[1] = Convert.ToByte("138");
            //sendobj8.Data[2] = Convert.ToByte("00");
            //sendobj8.Data[3] = Convert.ToByte("00");
            //sendobj8.Data[4] = Convert.ToByte("00");
            //sendobj8.Data[5] = Convert.ToByte("00");
            //sendobj8.Data[6] = Convert.ToByte("00");
            //sendobj8.Data[7] = Convert.ToByte("00");
            //if (VCI_Transmit(m_devtype, m_devind, m_canind, ref sendobj8, 1) == 0)
            //{
            //    MessageBox.Show("发送失败", "错误",
            //            MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            //}
            //System.Threading.Thread.Sleep(1000);
            //VCI_CAN_OBJ sendobj10 = new VCI_CAN_OBJ();
            //sendobj10.ExternFlag = 1;
            //sendobj10.ID = System.Convert.ToUInt32("18000029", 16);
            //sendobj10.DataLen = 8;
            //sendobj10.Data[0] = Convert.ToByte("191");
            //sendobj10.Data[1] = Convert.ToByte("139");
            //sendobj10.Data[2] = Convert.ToByte("00");
            //sendobj10.Data[3] = Convert.ToByte("00");
            //sendobj10.Data[4] = Convert.ToByte("00");
            //sendobj10.Data[5] = Convert.ToByte("00");
            //sendobj10.Data[6] = Convert.ToByte("00");
            //sendobj10.Data[7] = Convert.ToByte("00");
            //if (VCI_Transmit(m_devtype, m_devind, m_canind, ref sendobj10, 1) == 0)
            //{
            //    MessageBox.Show("发送失败", "错误",
            //            MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            //}
            //System.Threading.Thread.Sleep(1000);
            callback1.ID3 = Convert.ToString((/*Convert.ToUInt32(temp[5]) * 16777216 + Convert.ToUInt32(temp[4]) * 65536 +*/ Convert.ToDouble(Convert.ToUInt32(temp[3]) * 256 + Convert.ToUInt32(temp[2]))) / 10);
            容量标定.Enabled = true;
        }
       unsafe private void 从控电池串数标定_Click(object sender, EventArgs e)
        {
            从控电池串数标定.Enabled = false;
            if (m_bOpen == 0)
                return;
            VCI_CAN_OBJ sendobj = new VCI_CAN_OBJ();
            sendobj.ExternFlag = 1;
            sendobj.ID = System.Convert.ToUInt32("18000029", 16);
            sendobj.DataLen = 8;
            sendobj.Data[0] = Convert.ToByte("127");
            sendobj.Data[1] = Convert.ToByte("01");
            sendobj.Data[2] = Convert.ToByte("00");
            sendobj.Data[3] = Convert.ToByte(Convert.ToDouble(从控电池串数.Text));
            sendobj.Data[4] = Convert.ToByte("00");
            sendobj.Data[5] = Convert.ToByte("00");
            sendobj.Data[6] = Convert.ToByte("00");
            sendobj.Data[7] = Convert.ToByte("00");
            if (VCI_Transmit(m_devtype, m_devind, m_canind, ref sendobj, 1) == 0)
            {
                MessageBox.Show("发送失败", "错误",
                        MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
            System.Threading.Thread.Sleep(500);
            VCI_CAN_OBJ sendobj1 = new VCI_CAN_OBJ();
            sendobj1.ExternFlag = 1;
            sendobj1.ID = System.Convert.ToUInt32("18000029", 16);
            sendobj1.DataLen = 8;
            sendobj1.Data[0] = Convert.ToByte("191");
            sendobj1.Data[1] = Convert.ToByte("01");
            sendobj1.Data[2] = Convert.ToByte("00");
            sendobj1.Data[3] = Convert.ToByte("00");
            sendobj1.Data[4] = Convert.ToByte("00");
            sendobj1.Data[5] = Convert.ToByte("00");
            sendobj1.Data[6] = Convert.ToByte("00");
            sendobj1.Data[7] = Convert.ToByte("00");
            if (VCI_Transmit(m_devtype, m_devind, m_canind, ref sendobj1, 1) == 0)
            {
                MessageBox.Show("发送失败", "错误",
                        MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
            System.Threading.Thread.Sleep(1000);
            callback1.ID4 = temp[6]; 从控电池串数标定.Enabled = true;
        }
       unsafe private void 从控温度传感器数标定_Click(object sender, EventArgs e)
        {
            从控温度传感器数标定.Enabled = false;
            if (m_bOpen == 0)
                return;
            VCI_CAN_OBJ sendobj = new VCI_CAN_OBJ();
            sendobj.ExternFlag = 1;
            sendobj.ID = System.Convert.ToUInt32("18000029", 16);
            sendobj.DataLen = 8;
            sendobj.Data[0] = Convert.ToByte("127");
            sendobj.Data[1] = Convert.ToByte("31");
            sendobj.Data[2] = Convert.ToByte("00");
            sendobj.Data[3] = Convert.ToByte(Convert.ToUInt32(从控温度传感器数.Text));
            sendobj.Data[4] = Convert.ToByte("00");
            sendobj.Data[5] = Convert.ToByte("00");
            sendobj.Data[6] = Convert.ToByte("00");
            sendobj.Data[7] = Convert.ToByte("00");
            if (VCI_Transmit(m_devtype, m_devind, m_canind, ref sendobj, 1) == 0)
            {
                MessageBox.Show("发送失败", "错误",
                        MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
            System.Threading.Thread.Sleep(500);
            VCI_CAN_OBJ sendobj1 = new VCI_CAN_OBJ();
            sendobj1.ExternFlag = 1;
            sendobj1.ID = System.Convert.ToUInt32("18000029", 16);
            sendobj1.DataLen = 8;
            sendobj1.Data[0] = Convert.ToByte("191");
            sendobj1.Data[1] = Convert.ToByte("31");
            sendobj1.Data[2] = Convert.ToByte("00");
            sendobj1.Data[3] = Convert.ToByte("00");
            sendobj1.Data[4] = Convert.ToByte("00");
            sendobj1.Data[5] = Convert.ToByte("00");
            sendobj1.Data[6] = Convert.ToByte("00");
            sendobj1.Data[7] = Convert.ToByte("00");
            if (VCI_Transmit(m_devtype, m_devind, m_canind, ref sendobj1, 1) == 0)
            {
                MessageBox.Show("发送失败", "错误",
                        MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
            System.Threading.Thread.Sleep(1000);
            callback1.ID11 = temp[13];
            从控温度传感器数标定.Enabled = true;

        }
       unsafe private void 单体满电电压标定_Click(object sender, EventArgs e)
        {
            单体满电电压标定.Enabled = false;
            if (m_bOpen == 0)
                return;
            VCI_CAN_OBJ sendobj = new VCI_CAN_OBJ();
            sendobj.ExternFlag = 1;
            sendobj.ID = System.Convert.ToUInt32("18000029", 16);
            sendobj.DataLen = 8;
            sendobj.Data[0] = Convert.ToByte("127");
            sendobj.Data[1] = Convert.ToByte("140");
            sendobj.Data[2] = Convert.ToByte("00");
            sendobj.Data[3] = Convert.ToByte((Convert.ToUInt32(Convert.ToSingle(单体满电电压.Text) * 1000)) % 256);
            sendobj.Data[4] = Convert.ToByte("00");
            sendobj.Data[5] = Convert.ToByte("00");
            sendobj.Data[6] = Convert.ToByte("00");
            sendobj.Data[7] = Convert.ToByte("00");
            if (VCI_Transmit(m_devtype, m_devind, m_canind, ref sendobj, 1) == 0)
            {
                MessageBox.Show("发送失败", "错误",
                        MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
            System.Threading.Thread.Sleep(500);
            if (m_bOpen == 0)
                return;
            VCI_CAN_OBJ sendobj1 = new VCI_CAN_OBJ();
            sendobj1.ExternFlag = 1;
            sendobj1.ID = System.Convert.ToUInt32("18000029", 16);
            sendobj1.DataLen = 8;
            sendobj1.Data[0] = Convert.ToByte("127");
            sendobj1.Data[1] = Convert.ToByte("141");
            sendobj1.Data[2] = Convert.ToByte("00");
            sendobj1.Data[3] = Convert.ToByte((Convert.ToUInt32(Convert.ToSingle(单体满电电压.Text) * 1000)) / 256);
            sendobj1.Data[4] = Convert.ToByte("00");
            sendobj1.Data[5] = Convert.ToByte("00");
            sendobj1.Data[6] = Convert.ToByte("00");
            sendobj1.Data[7] = Convert.ToByte("00");
            if (VCI_Transmit(m_devtype, m_devind, m_canind, ref sendobj1, 1) == 0)
            {
                MessageBox.Show("发送失败", "错误",
                        MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
            System.Threading.Thread.Sleep(500);
            VCI_CAN_OBJ sendobj6 = new VCI_CAN_OBJ();
            sendobj6.ExternFlag = 1;
            sendobj6.ID = System.Convert.ToUInt32("18000029", 16);
            sendobj6.DataLen = 8;
            sendobj6.Data[0] = Convert.ToByte("191");
            sendobj6.Data[1] = Convert.ToByte("140");
            sendobj6.Data[2] = Convert.ToByte("00");
            sendobj6.Data[3] = Convert.ToByte("00");
            sendobj6.Data[4] = Convert.ToByte("00");
            sendobj6.Data[5] = Convert.ToByte("00");
            sendobj6.Data[6] = Convert.ToByte("00");
            sendobj6.Data[7] = Convert.ToByte("00");
            if (VCI_Transmit(m_devtype, m_devind, m_canind, ref sendobj6, 1) == 0)
            {
                MessageBox.Show("发送失败", "错误",
                        MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
            System.Threading.Thread.Sleep(1000);
            VCI_CAN_OBJ sendobj7 = new VCI_CAN_OBJ();
            sendobj7.ExternFlag = 1;
            sendobj7.ID = System.Convert.ToUInt32("18000029", 16);
            sendobj7.DataLen = 8;
            sendobj7.Data[0] = Convert.ToByte("191");
            sendobj7.Data[1] = Convert.ToByte("141");
            sendobj7.Data[2] = Convert.ToByte("00");
            sendobj7.Data[3] = Convert.ToByte("00");
            sendobj7.Data[4] = Convert.ToByte("00");
            sendobj7.Data[5] = Convert.ToByte("00");
            sendobj7.Data[6] = Convert.ToByte("00");
            sendobj7.Data[7] = Convert.ToByte("00");
            if (VCI_Transmit(m_devtype, m_devind, m_canind, ref sendobj7, 1) == 0)
            {
                MessageBox.Show("发送失败", "错误",
                        MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
            System.Threading.Thread.Sleep(1000);
            callback1.ID12 = temp[14];
            单体满电电压标定.Enabled = true;
        }
       unsafe private void 低压报警标定_Click(object sender, EventArgs e)
        {
            低压报警标定.Enabled = false;
            if (m_bOpen == 0)
                return;
            VCI_CAN_OBJ sendobj = new VCI_CAN_OBJ();
            sendobj.ExternFlag = 1;
            sendobj.ID = System.Convert.ToUInt32("18000029", 16);
            sendobj.DataLen = 8;
            sendobj.Data[0] = Convert.ToByte("127");
            sendobj.Data[1] = Convert.ToByte("81");
            sendobj.Data[2] = Convert.ToByte("00");
            sendobj.Data[3] = Convert.ToByte((Convert.ToUInt32(Convert.ToSingle(低压报警1.Text) * 1000)) % 256);
            sendobj.Data[4] = Convert.ToByte("00");
            sendobj.Data[5] = Convert.ToByte("00");
            sendobj.Data[6] = Convert.ToByte("00");
            sendobj.Data[7] = Convert.ToByte("00");
            if (VCI_Transmit(m_devtype, m_devind, m_canind, ref sendobj, 1) == 0)
            {
                MessageBox.Show("发送失败", "错误",
                        MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
            System.Threading.Thread.Sleep(500);
            if (m_bOpen == 0)
                return;
            VCI_CAN_OBJ sendobj1 = new VCI_CAN_OBJ();
            sendobj1.ExternFlag = 1;
            sendobj1.ID = System.Convert.ToUInt32("18000029", 16);
            sendobj1.DataLen = 8;
            sendobj1.Data[0] = Convert.ToByte("127");
            sendobj1.Data[1] = Convert.ToByte("82");
            sendobj1.Data[2] = Convert.ToByte("00");
            sendobj1.Data[3] = Convert.ToByte((Convert.ToUInt32(Convert.ToSingle(低压报警1.Text) * 1000)) / 256);
            sendobj1.Data[4] = Convert.ToByte("00");
            sendobj1.Data[5] = Convert.ToByte("00");
            sendobj1.Data[6] = Convert.ToByte("00");
            sendobj1.Data[7] = Convert.ToByte("00");
            if (VCI_Transmit(m_devtype, m_devind, m_canind, ref sendobj1, 1) == 0)
            {
                MessageBox.Show("发送失败", "错误",
                        MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
            System.Threading.Thread.Sleep(500);
            if (m_bOpen == 0)
                return;
            VCI_CAN_OBJ sendobj2 = new VCI_CAN_OBJ();
            sendobj2.ExternFlag = 1;
            sendobj2.ID = System.Convert.ToUInt32("18000029", 16);
            sendobj2.DataLen = 8;
            sendobj2.Data[0] = Convert.ToByte("127");
            sendobj2.Data[1] = Convert.ToByte("83");
            sendobj2.Data[2] = Convert.ToByte("00");
            sendobj2.Data[3] = Convert.ToByte((Convert.ToUInt32(Convert.ToSingle(低压报警2.Text) * 1000)) % 256);
            sendobj2.Data[4] = Convert.ToByte("00");
            sendobj2.Data[5] = Convert.ToByte("00");
            sendobj2.Data[6] = Convert.ToByte("00");
            sendobj2.Data[7] = Convert.ToByte("00");
            if (VCI_Transmit(m_devtype, m_devind, m_canind, ref sendobj2, 1) == 0)
            {
                MessageBox.Show("发送失败", "错误",
                        MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
            System.Threading.Thread.Sleep(500);
            if (m_bOpen == 0)
                return;
            VCI_CAN_OBJ sendobj3 = new VCI_CAN_OBJ();
            sendobj3.ExternFlag = 1;
            sendobj3.ID = System.Convert.ToUInt32("18000029", 16);
            sendobj3.DataLen = 8;
            sendobj3.Data[0] = Convert.ToByte("127");
            sendobj3.Data[1] = Convert.ToByte("84");
            sendobj3.Data[2] = Convert.ToByte("00");
            sendobj3.Data[3] = Convert.ToByte((Convert.ToUInt32(Convert.ToSingle(低压报警2.Text) * 1000)) / 256);
            sendobj3.Data[4] = Convert.ToByte("00");
            sendobj3.Data[5] = Convert.ToByte("00");
            sendobj3.Data[6] = Convert.ToByte("00");
            sendobj3.Data[7] = Convert.ToByte("00");
            if (VCI_Transmit(m_devtype, m_devind, m_canind, ref sendobj3, 1) == 0)
            {
                MessageBox.Show("发送失败", "错误",
                        MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
            System.Threading.Thread.Sleep(500);
            if (m_bOpen == 0)
                return;
            VCI_CAN_OBJ sendobj4 = new VCI_CAN_OBJ();
            sendobj4.ExternFlag = 1;
            sendobj4.ID = System.Convert.ToUInt32("18000029", 16);
            sendobj4.DataLen = 8;
            sendobj4.Data[0] = Convert.ToByte("127");
            sendobj4.Data[1] = Convert.ToByte("85");
            sendobj4.Data[2] = Convert.ToByte("00");
            sendobj4.Data[3] = Convert.ToByte((Convert.ToUInt32(Convert.ToSingle(低压报警3.Text) * 1000)) % 256);
            sendobj4.Data[4] = Convert.ToByte("00");
            sendobj4.Data[5] = Convert.ToByte("00");
            sendobj4.Data[6] = Convert.ToByte("00");
            sendobj4.Data[7] = Convert.ToByte("00");
            if (VCI_Transmit(m_devtype, m_devind, m_canind, ref sendobj4, 1) == 0)
            {
                MessageBox.Show("发送失败", "错误",
                        MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
            System.Threading.Thread.Sleep(500);
            if (m_bOpen == 0)
                return;
            VCI_CAN_OBJ sendobj5 = new VCI_CAN_OBJ();
            sendobj5.ExternFlag = 1;
            sendobj5.ID = System.Convert.ToUInt32("18000029", 16);
            sendobj5.DataLen = 8;
            sendobj5.Data[0] = Convert.ToByte("127");
            sendobj5.Data[1] = Convert.ToByte("86");
            sendobj5.Data[2] = Convert.ToByte("00");
            sendobj5.Data[3] = Convert.ToByte((Convert.ToUInt32(Convert.ToSingle(低压报警3.Text) * 1000)) / 256);
            sendobj5.Data[4] = Convert.ToByte("00");
            sendobj5.Data[5] = Convert.ToByte("00");
            sendobj5.Data[6] = Convert.ToByte("00");
            sendobj5.Data[7] = Convert.ToByte("00");
            if (VCI_Transmit(m_devtype, m_devind, m_canind, ref sendobj5, 1) == 0)
            {
                MessageBox.Show("发送失败", "错误",
                        MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
            System.Threading.Thread.Sleep(500);
            VCI_CAN_OBJ sendobj6 = new VCI_CAN_OBJ();
            sendobj6.ExternFlag = 1;
            sendobj6.ID = System.Convert.ToUInt32("18000029", 16);
            sendobj6.DataLen = 8;
            sendobj6.Data[0] = Convert.ToByte("191");
            sendobj6.Data[1] = Convert.ToByte("81");
            sendobj6.Data[2] = Convert.ToByte("00");
            sendobj6.Data[3] = Convert.ToByte("00");
            sendobj6.Data[4] = Convert.ToByte("00");
            sendobj6.Data[5] = Convert.ToByte("00");
            sendobj6.Data[6] = Convert.ToByte("00");
            sendobj6.Data[7] = Convert.ToByte("00");
            if (VCI_Transmit(m_devtype, m_devind, m_canind, ref sendobj6, 1) == 0)
            {
                MessageBox.Show("发送失败", "错误",
                        MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
            System.Threading.Thread.Sleep(1000);
            VCI_CAN_OBJ sendobj7 = new VCI_CAN_OBJ();
            sendobj7.ExternFlag = 1;
            sendobj7.ID = System.Convert.ToUInt32("18000029", 16);
            sendobj7.DataLen = 8;
            sendobj7.Data[0] = Convert.ToByte("191");
            sendobj7.Data[1] = Convert.ToByte("82");
            sendobj7.Data[2] = Convert.ToByte("00");
            sendobj7.Data[3] = Convert.ToByte("00");
            sendobj7.Data[4] = Convert.ToByte("00");
            sendobj7.Data[5] = Convert.ToByte("00");
            sendobj7.Data[6] = Convert.ToByte("00");
            sendobj7.Data[7] = Convert.ToByte("00");
            if (VCI_Transmit(m_devtype, m_devind, m_canind, ref sendobj7, 1) == 0)
            {
                MessageBox.Show("发送失败", "错误",
                        MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
            System.Threading.Thread.Sleep(1000);
            VCI_CAN_OBJ sendobj8 = new VCI_CAN_OBJ();
            sendobj8.ExternFlag = 1;
            sendobj8.ID = System.Convert.ToUInt32("18000029", 16);
            sendobj8.DataLen = 8;
            sendobj8.Data[0] = Convert.ToByte("191");
            sendobj8.Data[1] = Convert.ToByte("83");
            sendobj8.Data[2] = Convert.ToByte("00");
            sendobj8.Data[3] = Convert.ToByte("00");
            sendobj8.Data[4] = Convert.ToByte("00");
            sendobj8.Data[5] = Convert.ToByte("00");
            sendobj8.Data[6] = Convert.ToByte("00");
            sendobj8.Data[7] = Convert.ToByte("00");
            if (VCI_Transmit(m_devtype, m_devind, m_canind, ref sendobj8, 1) == 0)
            {
                MessageBox.Show("发送失败", "错误",
                        MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
            System.Threading.Thread.Sleep(1000);
            VCI_CAN_OBJ sendobj9 = new VCI_CAN_OBJ();
            sendobj9.ExternFlag = 1;
            sendobj9.ID = System.Convert.ToUInt32("18000029", 16);
            sendobj9.DataLen = 8;
            sendobj9.Data[0] = Convert.ToByte("191");
            sendobj9.Data[1] = Convert.ToByte("84");
            sendobj9.Data[2] = Convert.ToByte("00");
            sendobj9.Data[3] = Convert.ToByte("00");
            sendobj9.Data[4] = Convert.ToByte("00");
            sendobj9.Data[5] = Convert.ToByte("00");
            sendobj9.Data[6] = Convert.ToByte("00");
            sendobj9.Data[7] = Convert.ToByte("00");
            if (VCI_Transmit(m_devtype, m_devind, m_canind, ref sendobj9, 1) == 0)
            {
                MessageBox.Show("发送失败", "错误",
                        MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
            System.Threading.Thread.Sleep(1000);
            VCI_CAN_OBJ sendobj10 = new VCI_CAN_OBJ();
            sendobj10.ExternFlag = 1;
            sendobj10.ID = System.Convert.ToUInt32("18000029", 16);
            sendobj10.DataLen = 8;
            sendobj10.Data[0] = Convert.ToByte("191");
            sendobj10.Data[1] = Convert.ToByte("85");
            sendobj10.Data[2] = Convert.ToByte("00");
            sendobj10.Data[3] = Convert.ToByte("00");
            sendobj10.Data[4] = Convert.ToByte("00");
            sendobj10.Data[5] = Convert.ToByte("00");
            sendobj10.Data[6] = Convert.ToByte("00");
            sendobj10.Data[7] = Convert.ToByte("00");
            if (VCI_Transmit(m_devtype, m_devind, m_canind, ref sendobj10, 1) == 0)
            {
                MessageBox.Show("发送失败", "错误",
                        MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
            System.Threading.Thread.Sleep(1000);
            VCI_CAN_OBJ sendobj11 = new VCI_CAN_OBJ();
            sendobj11.ExternFlag = 1;
            sendobj11.ID = System.Convert.ToUInt32("18000029", 16);
            sendobj11.DataLen = 8;
            sendobj11.Data[0] = Convert.ToByte("191");
            sendobj11.Data[1] = Convert.ToByte("86");
            sendobj11.Data[2] = Convert.ToByte("00");
            sendobj11.Data[3] = Convert.ToByte("00");
            sendobj11.Data[4] = Convert.ToByte("00");
            sendobj11.Data[5] = Convert.ToByte("00");
            sendobj11.Data[6] = Convert.ToByte("00");
            sendobj11.Data[7] = Convert.ToByte("00");
            if (VCI_Transmit(m_devtype, m_devind, m_canind, ref sendobj11, 1) == 0)
            {
                MessageBox.Show("发送失败", "错误",
                        MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
            System.Threading.Thread.Sleep(1500);
            callback1.ID5 = temp[7]; callback1.ID6 = temp[8]; callback1.ID7 = temp[9];
            低压报警标定.Enabled = true;
        }
       unsafe private void 高压报警标定_Click(object sender, EventArgs e)
        {

            高压报警标定.Enabled = false;
            if (m_bOpen == 0)
                return;
            VCI_CAN_OBJ sendobj = new VCI_CAN_OBJ();
            sendobj.ExternFlag = 1;
            sendobj.ID = System.Convert.ToUInt32("18000029", 16);
            sendobj.DataLen = 8;
            sendobj.Data[0] = Convert.ToByte("127");
            sendobj.Data[1] = Convert.ToByte("87");
            sendobj.Data[2] = Convert.ToByte("00");
            sendobj.Data[3] = Convert.ToByte((Convert.ToUInt32(Convert.ToSingle(高压报警1.Text) * 1000)) % 256);
            sendobj.Data[4] = Convert.ToByte("00");
            sendobj.Data[5] = Convert.ToByte("00");
            sendobj.Data[6] = Convert.ToByte("00");
            sendobj.Data[7] = Convert.ToByte("00");
            if (VCI_Transmit(m_devtype, m_devind, m_canind, ref sendobj, 1) == 0)
            {
                MessageBox.Show("发送失败", "错误",
                        MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
            System.Threading.Thread.Sleep(500);
            if (m_bOpen == 0)
                return;
            VCI_CAN_OBJ sendobj1 = new VCI_CAN_OBJ();
            sendobj1.ExternFlag = 1;
            sendobj1.ID = System.Convert.ToUInt32("18000029", 16);
            sendobj1.DataLen = 8;
            sendobj1.Data[0] = Convert.ToByte("127");
            sendobj1.Data[1] = Convert.ToByte("88");
            sendobj1.Data[2] = Convert.ToByte("00");
            sendobj1.Data[3] = Convert.ToByte((Convert.ToUInt32(Convert.ToSingle(高压报警1.Text) * 1000)) / 256);
            sendobj1.Data[4] = Convert.ToByte("00");
            sendobj1.Data[5] = Convert.ToByte("00");
            sendobj1.Data[6] = Convert.ToByte("00");
            sendobj1.Data[7] = Convert.ToByte("00");
            if (VCI_Transmit(m_devtype, m_devind, m_canind, ref sendobj1, 1) == 0)
            {
                MessageBox.Show("发送失败", "错误",
                        MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
            System.Threading.Thread.Sleep(500);
            if (m_bOpen == 0)
                return;
            VCI_CAN_OBJ sendobj2 = new VCI_CAN_OBJ();
            sendobj2.ExternFlag = 1;
            sendobj2.ID = System.Convert.ToUInt32("18000029", 16);
            sendobj2.DataLen = 8;
            sendobj2.Data[0] = Convert.ToByte("127");
            sendobj2.Data[1] = Convert.ToByte("89");
            sendobj2.Data[2] = Convert.ToByte("00");
            sendobj2.Data[3] = Convert.ToByte((Convert.ToUInt32(Convert.ToSingle(高压报警2.Text) * 1000)) % 256);
            sendobj2.Data[4] = Convert.ToByte("00");
            sendobj2.Data[5] = Convert.ToByte("00");
            sendobj2.Data[6] = Convert.ToByte("00");
            sendobj2.Data[7] = Convert.ToByte("00");
            if (VCI_Transmit(m_devtype, m_devind, m_canind, ref sendobj2, 1) == 0)
            {
                MessageBox.Show("发送失败", "错误",
                        MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
            System.Threading.Thread.Sleep(500);
            if (m_bOpen == 0)
                return;
            VCI_CAN_OBJ sendobj3 = new VCI_CAN_OBJ();
            sendobj3.ExternFlag = 1;
            sendobj3.ID = System.Convert.ToUInt32("18000029", 16);
            sendobj3.DataLen = 8;
            sendobj3.Data[0] = Convert.ToByte("127");
            sendobj3.Data[1] = Convert.ToByte("90");
            sendobj3.Data[2] = Convert.ToByte("00");
            sendobj3.Data[3] = Convert.ToByte((Convert.ToUInt32(Convert.ToSingle(高压报警2.Text) * 1000)) / 256);
            sendobj3.Data[4] = Convert.ToByte("00");
            sendobj3.Data[5] = Convert.ToByte("00");
            sendobj3.Data[6] = Convert.ToByte("00");
            sendobj3.Data[7] = Convert.ToByte("00");
            if (VCI_Transmit(m_devtype, m_devind, m_canind, ref sendobj3, 1) == 0)
            {
                MessageBox.Show("发送失败", "错误",
                        MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
            System.Threading.Thread.Sleep(500);
            if (m_bOpen == 0)
                return;
            VCI_CAN_OBJ sendobj4 = new VCI_CAN_OBJ();
            sendobj4.ExternFlag = 1;
            sendobj4.ID = System.Convert.ToUInt32("18000029", 16);
            sendobj4.DataLen = 8;
            sendobj4.Data[0] = Convert.ToByte("127");
            sendobj4.Data[1] = Convert.ToByte("91");
            sendobj4.Data[2] = Convert.ToByte("00");
            sendobj4.Data[3] = Convert.ToByte((Convert.ToUInt32(Convert.ToSingle(高压报警3.Text) * 1000)) % 256);
            sendobj4.Data[4] = Convert.ToByte("00");
            sendobj4.Data[5] = Convert.ToByte("00");
            sendobj4.Data[6] = Convert.ToByte("00");
            sendobj4.Data[7] = Convert.ToByte("00");
            if (VCI_Transmit(m_devtype, m_devind, m_canind, ref sendobj4, 1) == 0)
            {
                MessageBox.Show("发送失败", "错误",
                        MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
            System.Threading.Thread.Sleep(500);
            if (m_bOpen == 0)
                return;
            VCI_CAN_OBJ sendobj5 = new VCI_CAN_OBJ();
            sendobj5.ExternFlag = 1;
            sendobj5.ID = System.Convert.ToUInt32("18000029", 16);
            sendobj5.DataLen = 8;
            sendobj5.Data[0] = Convert.ToByte("127");
            sendobj5.Data[1] = Convert.ToByte("92");
            sendobj5.Data[2] = Convert.ToByte("00");
            sendobj5.Data[3] = Convert.ToByte((Convert.ToUInt32(Convert.ToSingle(高压报警3.Text) * 1000)) / 256);
            sendobj5.Data[4] = Convert.ToByte("00");
            sendobj5.Data[5] = Convert.ToByte("00");
            sendobj5.Data[6] = Convert.ToByte("00");
            sendobj5.Data[7] = Convert.ToByte("00");
            if (VCI_Transmit(m_devtype, m_devind, m_canind, ref sendobj5, 1) == 0)
            {
                MessageBox.Show("发送失败", "错误",
                        MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
            System.Threading.Thread.Sleep(500);
            VCI_CAN_OBJ sendobj6 = new VCI_CAN_OBJ();
            sendobj6.ExternFlag = 1;
            sendobj6.ID = System.Convert.ToUInt32("18000029", 16);
            sendobj6.DataLen = 8;
            sendobj6.Data[0] = Convert.ToByte("191");
            sendobj6.Data[1] = Convert.ToByte("87");
            sendobj6.Data[2] = Convert.ToByte("00");
            sendobj6.Data[3] = Convert.ToByte("00");
            sendobj6.Data[4] = Convert.ToByte("00");
            sendobj6.Data[5] = Convert.ToByte("00");
            sendobj6.Data[6] = Convert.ToByte("00");
            sendobj6.Data[7] = Convert.ToByte("00");
            if (VCI_Transmit(m_devtype, m_devind, m_canind, ref sendobj6, 1) == 0)
            {
                MessageBox.Show("发送失败", "错误",
                        MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
            System.Threading.Thread.Sleep(1000);
            VCI_CAN_OBJ sendobj7 = new VCI_CAN_OBJ();
            sendobj7.ExternFlag = 1;
            sendobj7.ID = System.Convert.ToUInt32("18000029", 16);
            sendobj7.DataLen = 8;
            sendobj7.Data[0] = Convert.ToByte("191");
            sendobj7.Data[1] = Convert.ToByte("88");
            sendobj7.Data[2] = Convert.ToByte("00");
            sendobj7.Data[3] = Convert.ToByte("00");
            sendobj7.Data[4] = Convert.ToByte("00");
            sendobj7.Data[5] = Convert.ToByte("00");
            sendobj7.Data[6] = Convert.ToByte("00");
            sendobj7.Data[7] = Convert.ToByte("00");
            if (VCI_Transmit(m_devtype, m_devind, m_canind, ref sendobj7, 1) == 0)
            {
                MessageBox.Show("发送失败", "错误",
                        MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
            System.Threading.Thread.Sleep(1000);
            VCI_CAN_OBJ sendobj8 = new VCI_CAN_OBJ();
            sendobj8.ExternFlag = 1;
            sendobj8.ID = System.Convert.ToUInt32("18000029", 16);
            sendobj8.DataLen = 8;
            sendobj8.Data[0] = Convert.ToByte("191");
            sendobj8.Data[1] = Convert.ToByte("89");
            sendobj8.Data[2] = Convert.ToByte("00");
            sendobj8.Data[3] = Convert.ToByte("00");
            sendobj8.Data[4] = Convert.ToByte("00");
            sendobj8.Data[5] = Convert.ToByte("00");
            sendobj8.Data[6] = Convert.ToByte("00");
            sendobj8.Data[7] = Convert.ToByte("00");
            if (VCI_Transmit(m_devtype, m_devind, m_canind, ref sendobj8, 1) == 0)
            {
                MessageBox.Show("发送失败", "错误",
                        MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
            System.Threading.Thread.Sleep(1000);
            VCI_CAN_OBJ sendobj9 = new VCI_CAN_OBJ();
            sendobj9.ExternFlag = 1;
            sendobj9.ID = System.Convert.ToUInt32("18000029", 16);
            sendobj9.DataLen = 8;
            sendobj9.Data[0] = Convert.ToByte("191");
            sendobj9.Data[1] = Convert.ToByte("90");
            sendobj9.Data[2] = Convert.ToByte("00");
            sendobj9.Data[3] = Convert.ToByte("00");
            sendobj9.Data[4] = Convert.ToByte("00");
            sendobj9.Data[5] = Convert.ToByte("00");
            sendobj9.Data[6] = Convert.ToByte("00");
            sendobj9.Data[7] = Convert.ToByte("00");
            if (VCI_Transmit(m_devtype, m_devind, m_canind, ref sendobj9, 1) == 0)
            {
                MessageBox.Show("发送失败", "错误",
                        MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
            System.Threading.Thread.Sleep(1000);
            VCI_CAN_OBJ sendobj10 = new VCI_CAN_OBJ();
            sendobj10.ExternFlag = 1;
            sendobj10.ID = System.Convert.ToUInt32("18000029", 16);
            sendobj10.DataLen = 8;
            sendobj10.Data[0] = Convert.ToByte("191");
            sendobj10.Data[1] = Convert.ToByte("91");
            sendobj10.Data[2] = Convert.ToByte("00");
            sendobj10.Data[3] = Convert.ToByte("00");
            sendobj10.Data[4] = Convert.ToByte("00");
            sendobj10.Data[5] = Convert.ToByte("00");
            sendobj10.Data[6] = Convert.ToByte("00");
            sendobj10.Data[7] = Convert.ToByte("00");
            if (VCI_Transmit(m_devtype, m_devind, m_canind, ref sendobj10, 1) == 0)
            {
                MessageBox.Show("发送失败", "错误",
                        MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
            System.Threading.Thread.Sleep(1000);
            VCI_CAN_OBJ sendobj11 = new VCI_CAN_OBJ();
            sendobj11.ExternFlag = 1;
            sendobj11.ID = System.Convert.ToUInt32("18000029", 16);
            sendobj11.DataLen = 8;
            sendobj11.Data[0] = Convert.ToByte("191");
            sendobj11.Data[1] = Convert.ToByte("92");
            sendobj11.Data[2] = Convert.ToByte("00");
            sendobj11.Data[3] = Convert.ToByte("00");
            sendobj11.Data[4] = Convert.ToByte("00");
            sendobj11.Data[5] = Convert.ToByte("00");
            sendobj11.Data[6] = Convert.ToByte("00");
            sendobj11.Data[7] = Convert.ToByte("00");
            if (VCI_Transmit(m_devtype, m_devind, m_canind, ref sendobj11, 1) == 0)
            {
                MessageBox.Show("发送失败", "错误",
                        MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
            System.Threading.Thread.Sleep(1500);
            callback1.ID8 = temp[10]; callback1.ID9 = temp[11]; callback1.ID10 = temp[12];
            高压报警标定.Enabled = true;
        }
       unsafe private void 电池总个数标定_Click(object sender, EventArgs e)
        {
            电池总个数标定.Enabled = false;
            if (m_bOpen == 0)
                return;
            VCI_CAN_OBJ sendobj = new VCI_CAN_OBJ();
            sendobj.ExternFlag = 1;
            sendobj.ID = System.Convert.ToUInt32("18000029", 16);
            sendobj.DataLen = 8;
            sendobj.Data[0] = Convert.ToByte("127");
            sendobj.Data[1] = Convert.ToByte("21");
            sendobj.Data[2] = Convert.ToByte("00");
            sendobj.Data[3] = Convert.ToByte(Convert.ToUInt32(电池总个数.Text));
            sendobj.Data[4] = Convert.ToByte("00");
            sendobj.Data[5] = Convert.ToByte("00");
            sendobj.Data[6] = Convert.ToByte("00");
            sendobj.Data[7] = Convert.ToByte("00");
            if (VCI_Transmit(m_devtype, m_devind, m_canind, ref sendobj, 1) == 0)
            {
                MessageBox.Show("发送失败", "错误",
                        MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
            System.Threading.Thread.Sleep(500);
            VCI_CAN_OBJ sendobj1 = new VCI_CAN_OBJ();
            sendobj1.ExternFlag = 1;
            sendobj1.ID = System.Convert.ToUInt32("18000029", 16);
            sendobj1.DataLen = 8;
            sendobj1.Data[0] = Convert.ToByte("191");
            sendobj1.Data[1] = Convert.ToByte("21");
            sendobj1.Data[2] = Convert.ToByte("00");
            sendobj1.Data[3] = Convert.ToByte("00");
            sendobj1.Data[4] = Convert.ToByte("00");
            sendobj1.Data[5] = Convert.ToByte("00");
            sendobj1.Data[6] = Convert.ToByte("00");
            sendobj1.Data[7] = Convert.ToByte("00");
            if (VCI_Transmit(m_devtype, m_devind, m_canind, ref sendobj1, 1) == 0)
            {
                MessageBox.Show("发送失败", "错误",
                        MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
            System.Threading.Thread.Sleep(1000);
            callback1.ID13 = temp[15];
            电池总个数标定.Enabled = true;
        }
       unsafe private void 放电低温标定_Click(object sender, EventArgs e)
        {
            放电低温标定.Enabled = false;
            if (m_bOpen == 0)
                return;
            VCI_CAN_OBJ sendobj = new VCI_CAN_OBJ();
            sendobj.ExternFlag = 1;
            sendobj.ID = System.Convert.ToUInt32("18000029", 16);
            sendobj.DataLen = 8;
            sendobj.Data[0] = Convert.ToByte("127");
            sendobj.Data[1] = Convert.ToByte("25");
            sendobj.Data[2] = Convert.ToByte("00");
            sendobj.Data[3] = Convert.ToByte((Convert.ToInt64(放电低温1.Text))+40);
            sendobj.Data[4] = Convert.ToByte("00");
            sendobj.Data[5] = Convert.ToByte("00");
            sendobj.Data[6] = Convert.ToByte("00");
            sendobj.Data[7] = Convert.ToByte("00");
            if (VCI_Transmit(m_devtype, m_devind, m_canind, ref sendobj, 1) == 0)
            {
                MessageBox.Show("发送失败", "错误",
                        MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
            System.Threading.Thread.Sleep(500);
            if (m_bOpen == 0)
                return;
            VCI_CAN_OBJ sendobj1 = new VCI_CAN_OBJ();
            sendobj1.ExternFlag = 1;
            sendobj1.ID = System.Convert.ToUInt32("18000029", 16);
            sendobj1.DataLen = 8;
            sendobj1.Data[0] = Convert.ToByte("127");
            sendobj1.Data[1] = Convert.ToByte("26");
            sendobj1.Data[2] = Convert.ToByte("00");
            sendobj1.Data[3] = Convert.ToByte((Convert.ToInt64(放电低温2.Text)) + 40);
            sendobj1.Data[4] = Convert.ToByte("00");
            sendobj1.Data[5] = Convert.ToByte("00");
            sendobj1.Data[6] = Convert.ToByte("00");
            sendobj1.Data[7] = Convert.ToByte("00");
            if (VCI_Transmit(m_devtype, m_devind, m_canind, ref sendobj1, 1) == 0)
            {
                MessageBox.Show("发送失败", "错误",
                        MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
            System.Threading.Thread.Sleep(500);
            if (m_bOpen == 0)
                return;
            VCI_CAN_OBJ sendobj2 = new VCI_CAN_OBJ();
            sendobj2.ExternFlag = 1;
            sendobj2.ID = System.Convert.ToUInt32("18000029", 16);
            sendobj2.DataLen = 8;
            sendobj2.Data[0] = Convert.ToByte("127");
            sendobj2.Data[1] = Convert.ToByte("27");
            sendobj2.Data[2] = Convert.ToByte("00");
            sendobj2.Data[3] = Convert.ToByte((Convert.ToInt64(放电低温3.Text)) + 40);
            sendobj2.Data[4] = Convert.ToByte("00");
            sendobj2.Data[5] = Convert.ToByte("00");
            sendobj2.Data[6] = Convert.ToByte("00");
            sendobj2.Data[7] = Convert.ToByte("00");
            if (VCI_Transmit(m_devtype, m_devind, m_canind, ref sendobj2, 1) == 0)
            {
                MessageBox.Show("发送失败", "错误",
                        MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
            System.Threading.Thread.Sleep(500);
            VCI_CAN_OBJ sendobj6 = new VCI_CAN_OBJ();
            sendobj6.ExternFlag = 1;
            sendobj6.ID = System.Convert.ToUInt32("18000029", 16);
            sendobj6.DataLen = 8;
            sendobj6.Data[0] = Convert.ToByte("191");
            sendobj6.Data[1] = Convert.ToByte("25");
            sendobj6.Data[2] = Convert.ToByte("00");
            sendobj6.Data[3] = Convert.ToByte("00");
            sendobj6.Data[4] = Convert.ToByte("00");
            sendobj6.Data[5] = Convert.ToByte("00");
            sendobj6.Data[6] = Convert.ToByte("00");
            sendobj6.Data[7] = Convert.ToByte("00");
            if (VCI_Transmit(m_devtype, m_devind, m_canind, ref sendobj6, 1) == 0)
            {
                MessageBox.Show("发送失败", "错误",
                        MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
            System.Threading.Thread.Sleep(1000);
            VCI_CAN_OBJ sendobj7 = new VCI_CAN_OBJ();
            sendobj7.ExternFlag = 1;
            sendobj7.ID = System.Convert.ToUInt32("18000029", 16);
            sendobj7.DataLen = 8;
            sendobj7.Data[0] = Convert.ToByte("191");
            sendobj7.Data[1] = Convert.ToByte("26");
            sendobj7.Data[2] = Convert.ToByte("00");
            sendobj7.Data[3] = Convert.ToByte("00");
            sendobj7.Data[4] = Convert.ToByte("00");
            sendobj7.Data[5] = Convert.ToByte("00");
            sendobj7.Data[6] = Convert.ToByte("00");
            sendobj7.Data[7] = Convert.ToByte("00");
            if (VCI_Transmit(m_devtype, m_devind, m_canind, ref sendobj7, 1) == 0)
            {
                MessageBox.Show("发送失败", "错误",
                        MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
            System.Threading.Thread.Sleep(1000);
            VCI_CAN_OBJ sendobj8 = new VCI_CAN_OBJ();
            sendobj8.ExternFlag = 1;
            sendobj8.ID = System.Convert.ToUInt32("18000029", 16);
            sendobj8.DataLen = 8;
            sendobj8.Data[0] = Convert.ToByte("191");
            sendobj8.Data[1] = Convert.ToByte("27");
            sendobj8.Data[2] = Convert.ToByte("00");
            sendobj8.Data[3] = Convert.ToByte("00");
            sendobj8.Data[4] = Convert.ToByte("00");
            sendobj8.Data[5] = Convert.ToByte("00");
            sendobj8.Data[6] = Convert.ToByte("00");
            sendobj8.Data[7] = Convert.ToByte("00");
            if (VCI_Transmit(m_devtype, m_devind, m_canind, ref sendobj8, 1) == 0)
            {
                MessageBox.Show("发送失败", "错误",
                        MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
            System.Threading.Thread.Sleep(1500);
            callback1.ID14 = temp[16]; callback1.ID15 = temp[17]; callback1.ID16 = temp[18];
            放电低温标定.Enabled = true;
        }
        unsafe private void 最高温度报警标定_Click(object sender, EventArgs e)
        {
            最高温度报警标定.Enabled = false;
            if (m_bOpen == 0)
                return;
            VCI_CAN_OBJ sendobj = new VCI_CAN_OBJ();
            sendobj.ExternFlag = 1;
            sendobj.ID = System.Convert.ToUInt32("18000029", 16);
            sendobj.DataLen = 8;
            sendobj.Data[0] = Convert.ToByte("127");
            sendobj.Data[1] = Convert.ToByte("28");
            sendobj.Data[2] = Convert.ToByte("00");
            sendobj.Data[3] = Convert.ToByte((Convert.ToInt64(最高温度报警1.Text)) + 40);
            sendobj.Data[4] = Convert.ToByte("00");
            sendobj.Data[5] = Convert.ToByte("00");
            sendobj.Data[6] = Convert.ToByte("00");
            sendobj.Data[7] = Convert.ToByte("00");
            if (VCI_Transmit(m_devtype, m_devind, m_canind, ref sendobj, 1) == 0)
            {
                MessageBox.Show("发送失败", "错误",
                        MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
            System.Threading.Thread.Sleep(500);
            if (m_bOpen == 0)
                return;
            VCI_CAN_OBJ sendobj1 = new VCI_CAN_OBJ();
            sendobj1.ExternFlag = 1;
            sendobj1.ID = System.Convert.ToUInt32("18000029", 16);
            sendobj1.DataLen = 8;
            sendobj1.Data[0] = Convert.ToByte("127");
            sendobj1.Data[1] = Convert.ToByte("29");
            sendobj1.Data[2] = Convert.ToByte("00");
            sendobj1.Data[3] = Convert.ToByte((Convert.ToInt64(最高温度报警2.Text)) + 40);
            sendobj1.Data[4] = Convert.ToByte("00");
            sendobj1.Data[5] = Convert.ToByte("00");
            sendobj1.Data[6] = Convert.ToByte("00");
            sendobj1.Data[7] = Convert.ToByte("00");
            if (VCI_Transmit(m_devtype, m_devind, m_canind, ref sendobj1, 1) == 0)
            {
                MessageBox.Show("发送失败", "错误",
                        MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
            System.Threading.Thread.Sleep(500);
            if (m_bOpen == 0)
                return;
            VCI_CAN_OBJ sendobj2 = new VCI_CAN_OBJ();
            sendobj2.ExternFlag = 1;
            sendobj2.ID = System.Convert.ToUInt32("18000029", 16);
            sendobj2.DataLen = 8;
            sendobj2.Data[0] = Convert.ToByte("127");
            sendobj2.Data[1] = Convert.ToByte("30");
            sendobj2.Data[2] = Convert.ToByte("00");
            sendobj2.Data[3] = Convert.ToByte((Convert.ToInt64(最高温度报警3.Text)) + 40);
            sendobj2.Data[4] = Convert.ToByte("00");
            sendobj2.Data[5] = Convert.ToByte("00");
            sendobj2.Data[6] = Convert.ToByte("00");
            sendobj2.Data[7] = Convert.ToByte("00");
            if (VCI_Transmit(m_devtype, m_devind, m_canind, ref sendobj2, 1) == 0)
            {
                MessageBox.Show("发送失败", "错误",
                        MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
            System.Threading.Thread.Sleep(500);
            VCI_CAN_OBJ sendobj6 = new VCI_CAN_OBJ();
            sendobj6.ExternFlag = 1;
            sendobj6.ID = System.Convert.ToUInt32("18000029", 16);
            sendobj6.DataLen = 8;
            sendobj6.Data[0] = Convert.ToByte("191");
            sendobj6.Data[1] = Convert.ToByte("28");
            sendobj6.Data[2] = Convert.ToByte("00");
            sendobj6.Data[3] = Convert.ToByte("00");
            sendobj6.Data[4] = Convert.ToByte("00");
            sendobj6.Data[5] = Convert.ToByte("00");
            sendobj6.Data[6] = Convert.ToByte("00");
            sendobj6.Data[7] = Convert.ToByte("00");
            if (VCI_Transmit(m_devtype, m_devind, m_canind, ref sendobj6, 1) == 0)
            {
                MessageBox.Show("发送失败", "错误",
                        MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
            System.Threading.Thread.Sleep(1000);
            VCI_CAN_OBJ sendobj7 = new VCI_CAN_OBJ();
            sendobj7.ExternFlag = 1;
            sendobj7.ID = System.Convert.ToUInt32("18000029", 16);
            sendobj7.DataLen = 8;
            sendobj7.Data[0] = Convert.ToByte("191");
            sendobj7.Data[1] = Convert.ToByte("29");
            sendobj7.Data[2] = Convert.ToByte("00");
            sendobj7.Data[3] = Convert.ToByte("00");
            sendobj7.Data[4] = Convert.ToByte("00");
            sendobj7.Data[5] = Convert.ToByte("00");
            sendobj7.Data[6] = Convert.ToByte("00");
            sendobj7.Data[7] = Convert.ToByte("00");
            if (VCI_Transmit(m_devtype, m_devind, m_canind, ref sendobj7, 1) == 0)
            {
                MessageBox.Show("发送失败", "错误",
                        MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
            System.Threading.Thread.Sleep(1000);
            VCI_CAN_OBJ sendobj8 = new VCI_CAN_OBJ();
            sendobj8.ExternFlag = 1;
            sendobj8.ID = System.Convert.ToUInt32("18000029", 16);
            sendobj8.DataLen = 8;
            sendobj8.Data[0] = Convert.ToByte("191");
            sendobj8.Data[1] = Convert.ToByte("30");
            sendobj8.Data[2] = Convert.ToByte("00");
            sendobj8.Data[3] = Convert.ToByte("00");
            sendobj8.Data[4] = Convert.ToByte("00");
            sendobj8.Data[5] = Convert.ToByte("00");
            sendobj8.Data[6] = Convert.ToByte("00");
            sendobj8.Data[7] = Convert.ToByte("00");
            if (VCI_Transmit(m_devtype, m_devind, m_canind, ref sendobj8, 1) == 0)
            {
                MessageBox.Show("发送失败", "错误",
                        MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
            System.Threading.Thread.Sleep(1500);
            callback1.ID17 = temp[19]; callback1.ID18 = temp[20]; callback1.ID19 = temp[21];
            最高温度报警标定.Enabled = true;
        }
        unsafe private void 清除历史故障_Click(object sender, EventArgs e)
        {
            清除历史故障.Enabled = false;
            if (m_bOpen == 0)
                return;
            VCI_CAN_OBJ sendobj = new VCI_CAN_OBJ();
            sendobj.ExternFlag = 1;
            sendobj.ID = System.Convert.ToUInt32("18000029", 16);
            sendobj.DataLen = 8;
            sendobj.Data[0] = Convert.ToByte("127");
            sendobj.Data[1] = Convert.ToByte("190");
            sendobj.Data[2] = Convert.ToByte("00");
            sendobj.Data[3] = Convert.ToByte("00");
            sendobj.Data[4] = Convert.ToByte("00");
            sendobj.Data[5] = Convert.ToByte("00");
            sendobj.Data[6] = Convert.ToByte("00");
            sendobj.Data[7] = Convert.ToByte("00");
            if (VCI_Transmit(m_devtype, m_devind, m_canind, ref sendobj, 1) == 0)
            {
                MessageBox.Show("发送失败", "错误",
                        MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
            System.Threading.Thread.Sleep(500);
            VCI_CAN_OBJ sendobj1 = new VCI_CAN_OBJ();
            sendobj1.ExternFlag = 1;
            sendobj1.ID = System.Convert.ToUInt32("18000029", 16);
            sendobj1.DataLen = 8;
            sendobj1.Data[0] = Convert.ToByte("191");
            sendobj1.Data[1] = Convert.ToByte("190");
            sendobj1.Data[2] = Convert.ToByte("00");
            sendobj1.Data[3] = Convert.ToByte("00");
            sendobj1.Data[4] = Convert.ToByte("00");
            sendobj1.Data[5] = Convert.ToByte("00");
            sendobj1.Data[6] = Convert.ToByte("00");
            sendobj1.Data[7] = Convert.ToByte("00");
            if (VCI_Transmit(m_devtype, m_devind, m_canind, ref sendobj1, 1) == 0)
            {
                MessageBox.Show("发送失败", "错误",
                        MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
            System.Threading.Thread.Sleep(1000);
            //callback1.ID1 = temp[0];
            清除历史故障.Enabled = true;

        }
       unsafe private void 最大允许充电电流标定_Click(object sender, EventArgs e)
        {

            最大允许充电电流标定.Enabled = false;
            if (m_bOpen == 0)
                return;
            VCI_CAN_OBJ sendobj = new VCI_CAN_OBJ();
            sendobj.ExternFlag = 1;
            sendobj.ID = System.Convert.ToUInt32("18000029", 16);
            sendobj.DataLen = 8;
            sendobj.Data[0] = Convert.ToByte("127");
            sendobj.Data[1] = Convert.ToByte("93");
            sendobj.Data[2] = Convert.ToByte("00");
            sendobj.Data[3] = Convert.ToByte((Convert.ToUInt32(Convert.ToInt64(最大允许充电电流1.Text) * 10+32000)) % 256);
            sendobj.Data[4] = Convert.ToByte("00");
            sendobj.Data[5] = Convert.ToByte("00");
            sendobj.Data[6] = Convert.ToByte("00");
            sendobj.Data[7] = Convert.ToByte("00");
            if (VCI_Transmit(m_devtype, m_devind, m_canind, ref sendobj, 1) == 0)
            {
                MessageBox.Show("发送失败", "错误",
                        MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
            System.Threading.Thread.Sleep(500);
            if (m_bOpen == 0)
                return;
            VCI_CAN_OBJ sendobj1 = new VCI_CAN_OBJ();
            sendobj1.ExternFlag = 1;
            sendobj1.ID = System.Convert.ToUInt32("18000029", 16);
            sendobj1.DataLen = 8;
            sendobj1.Data[0] = Convert.ToByte("127");
            sendobj1.Data[1] = Convert.ToByte("94");
            sendobj1.Data[2] = Convert.ToByte("00");
            sendobj1.Data[3] = Convert.ToByte((Convert.ToUInt32(Convert.ToInt64(最大允许充电电流1.Text) * 10 + 32000)) / 256);
            sendobj1.Data[4] = Convert.ToByte("00");
            sendobj1.Data[5] = Convert.ToByte("00");
            sendobj1.Data[6] = Convert.ToByte("00");
            sendobj1.Data[7] = Convert.ToByte("00");
            if (VCI_Transmit(m_devtype, m_devind, m_canind, ref sendobj1, 1) == 0)
            {
                MessageBox.Show("发送失败", "错误",
                        MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
            System.Threading.Thread.Sleep(500);
            if (m_bOpen == 0)
                return;
            VCI_CAN_OBJ sendobj2 = new VCI_CAN_OBJ();
            sendobj2.ExternFlag = 1;
            sendobj2.ID = System.Convert.ToUInt32("18000029", 16);
            sendobj2.DataLen = 8;
            sendobj2.Data[0] = Convert.ToByte("127");
            sendobj2.Data[1] = Convert.ToByte("95");
            sendobj2.Data[2] = Convert.ToByte("00");
            sendobj2.Data[3] = Convert.ToByte((Convert.ToUInt32(Convert.ToInt64(最大允许充电电流2.Text) * 10 + 32000)) % 256);
            sendobj2.Data[4] = Convert.ToByte("00");
            sendobj2.Data[5] = Convert.ToByte("00");
            sendobj2.Data[6] = Convert.ToByte("00");
            sendobj2.Data[7] = Convert.ToByte("00");
            if (VCI_Transmit(m_devtype, m_devind, m_canind, ref sendobj2, 1) == 0)
            {
                MessageBox.Show("发送失败", "错误",
                        MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
            System.Threading.Thread.Sleep(500);
            if (m_bOpen == 0)
                return;
            VCI_CAN_OBJ sendobj3 = new VCI_CAN_OBJ();
            sendobj3.ExternFlag = 1;
            sendobj3.ID = System.Convert.ToUInt32("18000029", 16);
            sendobj3.DataLen = 8;
            sendobj3.Data[0] = Convert.ToByte("127");
            sendobj3.Data[1] = Convert.ToByte("96");
            sendobj3.Data[2] = Convert.ToByte("00");
            sendobj3.Data[3] = Convert.ToByte((Convert.ToUInt32(Convert.ToInt64(最大允许充电电流2.Text) * 10 + 32000))/ 256);
            sendobj3.Data[4] = Convert.ToByte("00");
            sendobj3.Data[5] = Convert.ToByte("00");
            sendobj3.Data[6] = Convert.ToByte("00");
            sendobj3.Data[7] = Convert.ToByte("00");
            if (VCI_Transmit(m_devtype, m_devind, m_canind, ref sendobj3, 1) == 0)
            {
                MessageBox.Show("发送失败", "错误",
                        MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
            System.Threading.Thread.Sleep(500);
            if (m_bOpen == 0)
                return;
            VCI_CAN_OBJ sendobj4 = new VCI_CAN_OBJ();
            sendobj4.ExternFlag = 1;
            sendobj4.ID = System.Convert.ToUInt32("18000029", 16);
            sendobj4.DataLen = 8;
            sendobj4.Data[0] = Convert.ToByte("127");
            sendobj4.Data[1] = Convert.ToByte("97");
            sendobj4.Data[2] = Convert.ToByte("00");
            sendobj4.Data[3] = Convert.ToByte((Convert.ToUInt32(Convert.ToInt64(最大允许充电电流3.Text) * 10 + 32000)) % 256);
            sendobj4.Data[4] = Convert.ToByte("00");
            sendobj4.Data[5] = Convert.ToByte("00");
            sendobj4.Data[6] = Convert.ToByte("00");
            sendobj4.Data[7] = Convert.ToByte("00");
            if (VCI_Transmit(m_devtype, m_devind, m_canind, ref sendobj4, 1) == 0)
            {
                MessageBox.Show("发送失败", "错误",
                        MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
            System.Threading.Thread.Sleep(500);
            if (m_bOpen == 0)
                return;
            VCI_CAN_OBJ sendobj5 = new VCI_CAN_OBJ();
            sendobj5.ExternFlag = 1;
            sendobj5.ID = System.Convert.ToUInt32("18000029", 16);
            sendobj5.DataLen = 8;
            sendobj5.Data[0] = Convert.ToByte("127");
            sendobj5.Data[1] = Convert.ToByte("98");
            sendobj5.Data[2] = Convert.ToByte("00");
            sendobj5.Data[3] = Convert.ToByte((Convert.ToUInt32(Convert.ToInt64(最大允许充电电流3.Text) * 10 + 32000)) / 256);
            sendobj5.Data[4] = Convert.ToByte("00");
            sendobj5.Data[5] = Convert.ToByte("00");
            sendobj5.Data[6] = Convert.ToByte("00");
            sendobj5.Data[7] = Convert.ToByte("00");
            if (VCI_Transmit(m_devtype, m_devind, m_canind, ref sendobj5, 1) == 0)
            {
                MessageBox.Show("发送失败", "错误",
                        MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
            System.Threading.Thread.Sleep(500);
            VCI_CAN_OBJ sendobj6 = new VCI_CAN_OBJ();
            sendobj6.ExternFlag = 1;
            sendobj6.ID = System.Convert.ToUInt32("18000029", 16);
            sendobj6.DataLen = 8;
            sendobj6.Data[0] = Convert.ToByte("191");
            sendobj6.Data[1] = Convert.ToByte("93");
            sendobj6.Data[2] = Convert.ToByte("00");
            sendobj6.Data[3] = Convert.ToByte("00");
            sendobj6.Data[4] = Convert.ToByte("00");
            sendobj6.Data[5] = Convert.ToByte("00");
            sendobj6.Data[6] = Convert.ToByte("00");
            sendobj6.Data[7] = Convert.ToByte("00");
            if (VCI_Transmit(m_devtype, m_devind, m_canind, ref sendobj6, 1) == 0)
            {
                MessageBox.Show("发送失败", "错误",
                        MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
            System.Threading.Thread.Sleep(1000);
            VCI_CAN_OBJ sendobj7 = new VCI_CAN_OBJ();
            sendobj7.ExternFlag = 1;
            sendobj7.ID = System.Convert.ToUInt32("18000029", 16);
            sendobj7.DataLen = 8;
            sendobj7.Data[0] = Convert.ToByte("191");
            sendobj7.Data[1] = Convert.ToByte("94");
            sendobj7.Data[2] = Convert.ToByte("00");
            sendobj7.Data[3] = Convert.ToByte("00");
            sendobj7.Data[4] = Convert.ToByte("00");
            sendobj7.Data[5] = Convert.ToByte("00");
            sendobj7.Data[6] = Convert.ToByte("00");
            sendobj7.Data[7] = Convert.ToByte("00");
            if (VCI_Transmit(m_devtype, m_devind, m_canind, ref sendobj7, 1) == 0)
            {
                MessageBox.Show("发送失败", "错误",
                        MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
            System.Threading.Thread.Sleep(1000);
            VCI_CAN_OBJ sendobj8 = new VCI_CAN_OBJ();
            sendobj8.ExternFlag = 1;
            sendobj8.ID = System.Convert.ToUInt32("18000029", 16);
            sendobj8.DataLen = 8;
            sendobj8.Data[0] = Convert.ToByte("191");
            sendobj8.Data[1] = Convert.ToByte("95");
            sendobj8.Data[2] = Convert.ToByte("00");
            sendobj8.Data[3] = Convert.ToByte("00");
            sendobj8.Data[4] = Convert.ToByte("00");
            sendobj8.Data[5] = Convert.ToByte("00");
            sendobj8.Data[6] = Convert.ToByte("00");
            sendobj8.Data[7] = Convert.ToByte("00");
            if (VCI_Transmit(m_devtype, m_devind, m_canind, ref sendobj8, 1) == 0)
            {
                MessageBox.Show("发送失败", "错误",
                        MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
            System.Threading.Thread.Sleep(1000);
            VCI_CAN_OBJ sendobj9 = new VCI_CAN_OBJ();
            sendobj9.ExternFlag = 1;
            sendobj9.ID = System.Convert.ToUInt32("18000029", 16);
            sendobj9.DataLen = 8;
            sendobj9.Data[0] = Convert.ToByte("191");
            sendobj9.Data[1] = Convert.ToByte("96");
            sendobj9.Data[2] = Convert.ToByte("00");
            sendobj9.Data[3] = Convert.ToByte("00");
            sendobj9.Data[4] = Convert.ToByte("00");
            sendobj9.Data[5] = Convert.ToByte("00");
            sendobj9.Data[6] = Convert.ToByte("00");
            sendobj9.Data[7] = Convert.ToByte("00");
            if (VCI_Transmit(m_devtype, m_devind, m_canind, ref sendobj9, 1) == 0)
            {
                MessageBox.Show("发送失败", "错误",
                        MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
            System.Threading.Thread.Sleep(1000);
            VCI_CAN_OBJ sendobj10 = new VCI_CAN_OBJ();
            sendobj10.ExternFlag = 1;
            sendobj10.ID = System.Convert.ToUInt32("18000029", 16);
            sendobj10.DataLen = 8;
            sendobj10.Data[0] = Convert.ToByte("191");
            sendobj10.Data[1] = Convert.ToByte("97");
            sendobj10.Data[2] = Convert.ToByte("00");
            sendobj10.Data[3] = Convert.ToByte("00");
            sendobj10.Data[4] = Convert.ToByte("00");
            sendobj10.Data[5] = Convert.ToByte("00");
            sendobj10.Data[6] = Convert.ToByte("00");
            sendobj10.Data[7] = Convert.ToByte("00");
            if (VCI_Transmit(m_devtype, m_devind, m_canind, ref sendobj10, 1) == 0)
            {
                MessageBox.Show("发送失败", "错误",
                        MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
            System.Threading.Thread.Sleep(1000);
            VCI_CAN_OBJ sendobj11 = new VCI_CAN_OBJ();
            sendobj11.ExternFlag = 1;
            sendobj11.ID = System.Convert.ToUInt32("18000029", 16);
            sendobj11.DataLen = 8;
            sendobj11.Data[0] = Convert.ToByte("191");
            sendobj11.Data[1] = Convert.ToByte("98");
            sendobj11.Data[2] = Convert.ToByte("00");
            sendobj11.Data[3] = Convert.ToByte("00");
            sendobj11.Data[4] = Convert.ToByte("00");
            sendobj11.Data[5] = Convert.ToByte("00");
            sendobj11.Data[6] = Convert.ToByte("00");
            sendobj11.Data[7] = Convert.ToByte("00");
            if (VCI_Transmit(m_devtype, m_devind, m_canind, ref sendobj11, 1) == 0)
            {
                MessageBox.Show("发送失败", "错误",
                        MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
            System.Threading.Thread.Sleep(1500);
            callback1.ID21 = temp[23]; callback1.ID22 = temp[24]; callback1.ID23 = temp[25];
            最大允许充电电流标定.Enabled = true;
        }
       unsafe private void 最大允许放电电流标定_Click(object sender, EventArgs e)
        {
            最大允许放电电流标定.Enabled = false;
            if (m_bOpen == 0)
                return;
            VCI_CAN_OBJ sendobj = new VCI_CAN_OBJ();
            sendobj.ExternFlag = 1;
            sendobj.ID = System.Convert.ToUInt32("18000029", 16);
            sendobj.DataLen = 8;
            sendobj.Data[0] = Convert.ToByte("127");
            sendobj.Data[1] = Convert.ToByte("99");
            sendobj.Data[2] = Convert.ToByte("00");
            sendobj.Data[3] = Convert.ToByte((Convert.ToInt64(Convert.ToInt64(最大允许放电电流1.Text) * 10 + 32000)) % 256);
            sendobj.Data[4] = Convert.ToByte("00");
            sendobj.Data[5] = Convert.ToByte("00");
            sendobj.Data[6] = Convert.ToByte("00");
            sendobj.Data[7] = Convert.ToByte("00");
            if (VCI_Transmit(m_devtype, m_devind, m_canind, ref sendobj, 1) == 0)
            {
                MessageBox.Show("发送失败", "错误",
                        MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
            System.Threading.Thread.Sleep(500);
            if (m_bOpen == 0)
                return;
            VCI_CAN_OBJ sendobj1 = new VCI_CAN_OBJ();
            sendobj1.ExternFlag = 1;
            sendobj1.ID = System.Convert.ToUInt32("18000029", 16);
            sendobj1.DataLen = 8;
            sendobj1.Data[0] = Convert.ToByte("127");
            sendobj1.Data[1] = Convert.ToByte("100");
            sendobj1.Data[2] = Convert.ToByte("00");
            sendobj1.Data[3] = Convert.ToByte((Convert.ToInt64(Convert.ToInt64(最大允许放电电流1.Text) * 10 + 32000)) / 256);
            sendobj1.Data[4] = Convert.ToByte("00");
            sendobj1.Data[5] = Convert.ToByte("00");
            sendobj1.Data[6] = Convert.ToByte("00");
            sendobj1.Data[7] = Convert.ToByte("00");
            if (VCI_Transmit(m_devtype, m_devind, m_canind, ref sendobj1, 1) == 0)
            {
                MessageBox.Show("发送失败", "错误",
                        MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
            System.Threading.Thread.Sleep(500);
            if (m_bOpen == 0)
                return;
            VCI_CAN_OBJ sendobj2 = new VCI_CAN_OBJ();
            sendobj2.ExternFlag = 1;
            sendobj2.ID = System.Convert.ToUInt32("18000029", 16);
            sendobj2.DataLen = 8;
            sendobj2.Data[0] = Convert.ToByte("127");
            sendobj2.Data[1] = Convert.ToByte("101");
            sendobj2.Data[2] = Convert.ToByte("00");
            sendobj2.Data[3] = Convert.ToByte((Convert.ToInt64(Convert.ToInt64(最大允许放电电流2.Text) * 10 + 32000)) % 256);
            sendobj2.Data[4] = Convert.ToByte("00");
            sendobj2.Data[5] = Convert.ToByte("00");
            sendobj2.Data[6] = Convert.ToByte("00");
            sendobj2.Data[7] = Convert.ToByte("00");
            if (VCI_Transmit(m_devtype, m_devind, m_canind, ref sendobj2, 1) == 0)
            {
                MessageBox.Show("发送失败", "错误",
                        MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
            System.Threading.Thread.Sleep(500);
            if (m_bOpen == 0)
                return;
            VCI_CAN_OBJ sendobj3 = new VCI_CAN_OBJ();
            sendobj3.ExternFlag = 1;
            sendobj3.ID = System.Convert.ToUInt32("18000029", 16);
            sendobj3.DataLen = 8;
            sendobj3.Data[0] = Convert.ToByte("127");
            sendobj3.Data[1] = Convert.ToByte("102");
            sendobj3.Data[2] = Convert.ToByte("00");
            sendobj3.Data[3] = Convert.ToByte((Convert.ToInt64(Convert.ToInt64(最大允许放电电流2.Text) * 10 + 32000)) / 256);
            sendobj3.Data[4] = Convert.ToByte("00");
            sendobj3.Data[5] = Convert.ToByte("00");
            sendobj3.Data[6] = Convert.ToByte("00");
            sendobj3.Data[7] = Convert.ToByte("00");
            if (VCI_Transmit(m_devtype, m_devind, m_canind, ref sendobj3, 1) == 0)
            {
                MessageBox.Show("发送失败", "错误",
                        MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
            System.Threading.Thread.Sleep(500);
            if (m_bOpen == 0)
                return;
            VCI_CAN_OBJ sendobj4 = new VCI_CAN_OBJ();
            sendobj4.ExternFlag = 1;
            sendobj4.ID = System.Convert.ToUInt32("18000029", 16);
            sendobj4.DataLen = 8;
            sendobj4.Data[0] = Convert.ToByte("127");
            sendobj4.Data[1] = Convert.ToByte("103");
            sendobj4.Data[2] = Convert.ToByte("00");
            sendobj4.Data[3] = Convert.ToByte((Convert.ToInt64(Convert.ToInt64(最大允许放电电流3.Text) * 10 + 32000)) % 256);
            sendobj4.Data[4] = Convert.ToByte("00");
            sendobj4.Data[5] = Convert.ToByte("00");
            sendobj4.Data[6] = Convert.ToByte("00");
            sendobj4.Data[7] = Convert.ToByte("00");
            if (VCI_Transmit(m_devtype, m_devind, m_canind, ref sendobj4, 1) == 0)
            {
                MessageBox.Show("发送失败", "错误",
                        MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
            System.Threading.Thread.Sleep(500);
            if (m_bOpen == 0)
                return;
            VCI_CAN_OBJ sendobj5 = new VCI_CAN_OBJ();
            sendobj5.ExternFlag = 1;
            sendobj5.ID = System.Convert.ToUInt32("18000029", 16);
            sendobj5.DataLen = 8;
            sendobj5.Data[0] = Convert.ToByte("127");
            sendobj5.Data[1] = Convert.ToByte("104");
            sendobj5.Data[2] = Convert.ToByte("00");
            sendobj5.Data[3] = Convert.ToByte((Convert.ToInt64(Convert.ToInt64(最大允许放电电流3.Text) * 10 + 32000)) / 256);
            sendobj5.Data[4] = Convert.ToByte("00");
            sendobj5.Data[5] = Convert.ToByte("00");
            sendobj5.Data[6] = Convert.ToByte("00");
            sendobj5.Data[7] = Convert.ToByte("00");
            if (VCI_Transmit(m_devtype, m_devind, m_canind, ref sendobj5, 1) == 0)
            {
                MessageBox.Show("发送失败", "错误",
                        MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
            System.Threading.Thread.Sleep(500);
            VCI_CAN_OBJ sendobj6 = new VCI_CAN_OBJ();
            sendobj6.ExternFlag = 1;
            sendobj6.ID = System.Convert.ToUInt32("18000029", 16);
            sendobj6.DataLen = 8;
            sendobj6.Data[0] = Convert.ToByte("191");
            sendobj6.Data[1] = Convert.ToByte("99");
            sendobj6.Data[2] = Convert.ToByte("00");
            sendobj6.Data[3] = Convert.ToByte("00");
            sendobj6.Data[4] = Convert.ToByte("00");
            sendobj6.Data[5] = Convert.ToByte("00");
            sendobj6.Data[6] = Convert.ToByte("00");
            sendobj6.Data[7] = Convert.ToByte("00");
            if (VCI_Transmit(m_devtype, m_devind, m_canind, ref sendobj6, 1) == 0)
            {
                MessageBox.Show("发送失败", "错误",
                        MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
            System.Threading.Thread.Sleep(1000);
            VCI_CAN_OBJ sendobj7 = new VCI_CAN_OBJ();
            sendobj7.ExternFlag = 1;
            sendobj7.ID = System.Convert.ToUInt32("18000029", 16);
            sendobj7.DataLen = 8;
            sendobj7.Data[0] = Convert.ToByte("191");
            sendobj7.Data[1] = Convert.ToByte("100");
            sendobj7.Data[2] = Convert.ToByte("00");
            sendobj7.Data[3] = Convert.ToByte("00");
            sendobj7.Data[4] = Convert.ToByte("00");
            sendobj7.Data[5] = Convert.ToByte("00");
            sendobj7.Data[6] = Convert.ToByte("00");
            sendobj7.Data[7] = Convert.ToByte("00");
            if (VCI_Transmit(m_devtype, m_devind, m_canind, ref sendobj7, 1) == 0)
            {
                MessageBox.Show("发送失败", "错误",
                        MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
            System.Threading.Thread.Sleep(1000);
            VCI_CAN_OBJ sendobj8 = new VCI_CAN_OBJ();
            sendobj8.ExternFlag = 1;
            sendobj8.ID = System.Convert.ToUInt32("18000029", 16);
            sendobj8.DataLen = 8;
            sendobj8.Data[0] = Convert.ToByte("191");
            sendobj8.Data[1] = Convert.ToByte("101");
            sendobj8.Data[2] = Convert.ToByte("00");
            sendobj8.Data[3] = Convert.ToByte("00");
            sendobj8.Data[4] = Convert.ToByte("00");
            sendobj8.Data[5] = Convert.ToByte("00");
            sendobj8.Data[6] = Convert.ToByte("00");
            sendobj8.Data[7] = Convert.ToByte("00");
            if (VCI_Transmit(m_devtype, m_devind, m_canind, ref sendobj8, 1) == 0)
            {
                MessageBox.Show("发送失败", "错误",
                        MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
            System.Threading.Thread.Sleep(1000);
            VCI_CAN_OBJ sendobj9 = new VCI_CAN_OBJ();
            sendobj9.ExternFlag = 1;
            sendobj9.ID = System.Convert.ToUInt32("18000029", 16);
            sendobj9.DataLen = 8;
            sendobj9.Data[0] = Convert.ToByte("191");
            sendobj9.Data[1] = Convert.ToByte("102");
            sendobj9.Data[2] = Convert.ToByte("00");
            sendobj9.Data[3] = Convert.ToByte("00");
            sendobj9.Data[4] = Convert.ToByte("00");
            sendobj9.Data[5] = Convert.ToByte("00");
            sendobj9.Data[6] = Convert.ToByte("00");
            sendobj9.Data[7] = Convert.ToByte("00");
            if (VCI_Transmit(m_devtype, m_devind, m_canind, ref sendobj9, 1) == 0)
            {
                MessageBox.Show("发送失败", "错误",
                        MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
            System.Threading.Thread.Sleep(1000);
            VCI_CAN_OBJ sendobj10 = new VCI_CAN_OBJ();
            sendobj10.ExternFlag = 1;
            sendobj10.ID = System.Convert.ToUInt32("18000029", 16);
            sendobj10.DataLen = 8;
            sendobj10.Data[0] = Convert.ToByte("191");
            sendobj10.Data[1] = Convert.ToByte("103");
            sendobj10.Data[2] = Convert.ToByte("00");
            sendobj10.Data[3] = Convert.ToByte("00");
            sendobj10.Data[4] = Convert.ToByte("00");
            sendobj10.Data[5] = Convert.ToByte("00");
            sendobj10.Data[6] = Convert.ToByte("00");
            sendobj10.Data[7] = Convert.ToByte("00");
            if (VCI_Transmit(m_devtype, m_devind, m_canind, ref sendobj10, 1) == 0)
            {
                MessageBox.Show("发送失败", "错误",
                        MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
            System.Threading.Thread.Sleep(1000);
            VCI_CAN_OBJ sendobj11 = new VCI_CAN_OBJ();
            sendobj11.ExternFlag = 1;
            sendobj11.ID = System.Convert.ToUInt32("18000029", 16);
            sendobj11.DataLen = 8;
            sendobj11.Data[0] = Convert.ToByte("191");
            sendobj11.Data[1] = Convert.ToByte("104");
            sendobj11.Data[2] = Convert.ToByte("00");
            sendobj11.Data[3] = Convert.ToByte("00");
            sendobj11.Data[4] = Convert.ToByte("00");
            sendobj11.Data[5] = Convert.ToByte("00");
            sendobj11.Data[6] = Convert.ToByte("00");
            sendobj11.Data[7] = Convert.ToByte("00");
            if (VCI_Transmit(m_devtype, m_devind, m_canind, ref sendobj11, 1) == 0)
            {
                MessageBox.Show("发送失败", "错误",
                        MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
            System.Threading.Thread.Sleep(1500);
            callback1.ID24 = temp[26]; callback1.ID25 = temp[27]; callback1.ID26 = temp[28];
            最大允许放电电流标定.Enabled = true;
        }
        unsafe private void 电压不均衡标定_Click(object sender, EventArgs e)
        {
            电压不均衡标定.Enabled = false;
            if (m_bOpen == 0)
                return;
            VCI_CAN_OBJ sendobj = new VCI_CAN_OBJ();
            sendobj.ExternFlag = 1;
            sendobj.ID = System.Convert.ToUInt32("18000029", 16);
            sendobj.DataLen = 8;
            sendobj.Data[0] = Convert.ToByte("127");
            sendobj.Data[1] = Convert.ToByte("105");
            sendobj.Data[2] = Convert.ToByte("00");
            sendobj.Data[3] = Convert.ToByte((Convert.ToUInt32((电压不均衡1.Text) )) % 256);
            sendobj.Data[4] = Convert.ToByte("00");
            sendobj.Data[5] = Convert.ToByte("00");
            sendobj.Data[6] = Convert.ToByte("00");
            sendobj.Data[7] = Convert.ToByte("00");
            if (VCI_Transmit(m_devtype, m_devind, m_canind, ref sendobj, 1) == 0)
            {
                MessageBox.Show("发送失败", "错误",
                        MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
            System.Threading.Thread.Sleep(500);
            if (m_bOpen == 0)
                return;
            VCI_CAN_OBJ sendobj1 = new VCI_CAN_OBJ();
            sendobj1.ExternFlag = 1;
            sendobj1.ID = System.Convert.ToUInt32("18000029", 16);
            sendobj1.DataLen = 8;
            sendobj1.Data[0] = Convert.ToByte("127");
            sendobj1.Data[1] = Convert.ToByte("106");
            sendobj1.Data[2] = Convert.ToByte("00");
            sendobj1.Data[3] = Convert.ToByte((Convert.ToUInt32((电压不均衡1.Text) )) / 256);
            sendobj1.Data[4] = Convert.ToByte("00");
            sendobj1.Data[5] = Convert.ToByte("00");
            sendobj1.Data[6] = Convert.ToByte("00");
            sendobj1.Data[7] = Convert.ToByte("00");
            if (VCI_Transmit(m_devtype, m_devind, m_canind, ref sendobj1, 1) == 0)
            {
                MessageBox.Show("发送失败", "错误",
                        MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
            System.Threading.Thread.Sleep(500);
            if (m_bOpen == 0)
                return;
            VCI_CAN_OBJ sendobj2 = new VCI_CAN_OBJ();
            sendobj2.ExternFlag = 1;
            sendobj2.ID = System.Convert.ToUInt32("18000029", 16);
            sendobj2.DataLen = 8;
            sendobj2.Data[0] = Convert.ToByte("127");
            sendobj2.Data[1] = Convert.ToByte("107");
            sendobj2.Data[2] = Convert.ToByte("00");
            sendobj2.Data[3] = Convert.ToByte((Convert.ToUInt32((电压不均衡2.Text) )) % 256);
            sendobj2.Data[4] = Convert.ToByte("00");
            sendobj2.Data[5] = Convert.ToByte("00");
            sendobj2.Data[6] = Convert.ToByte("00");
            sendobj2.Data[7] = Convert.ToByte("00");
            if (VCI_Transmit(m_devtype, m_devind, m_canind, ref sendobj2, 1) == 0)
            {
                MessageBox.Show("发送失败", "错误",
                        MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
            System.Threading.Thread.Sleep(500);
            if (m_bOpen == 0)
                return;
            VCI_CAN_OBJ sendobj3 = new VCI_CAN_OBJ();
            sendobj3.ExternFlag = 1;
            sendobj3.ID = System.Convert.ToUInt32("18000029", 16);
            sendobj3.DataLen = 8;
            sendobj3.Data[0] = Convert.ToByte("127");
            sendobj3.Data[1] = Convert.ToByte("108");
            sendobj3.Data[2] = Convert.ToByte("00");
            sendobj3.Data[3] = Convert.ToByte((Convert.ToUInt32((电压不均衡2.Text) )) / 256);
            sendobj3.Data[4] = Convert.ToByte("00");
            sendobj3.Data[5] = Convert.ToByte("00");
            sendobj3.Data[6] = Convert.ToByte("00");
            sendobj3.Data[7] = Convert.ToByte("00");
            if (VCI_Transmit(m_devtype, m_devind, m_canind, ref sendobj3, 1) == 0)
            {
                MessageBox.Show("发送失败", "错误",
                        MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
            System.Threading.Thread.Sleep(500);
            if (m_bOpen == 0)
                return;
            VCI_CAN_OBJ sendobj4 = new VCI_CAN_OBJ();
            sendobj4.ExternFlag = 1;
            sendobj4.ID = System.Convert.ToUInt32("18000029", 16);
            sendobj4.DataLen = 8;
            sendobj4.Data[0] = Convert.ToByte("127");
            sendobj4.Data[1] = Convert.ToByte("109");
            sendobj4.Data[2] = Convert.ToByte("00");
            sendobj4.Data[3] = Convert.ToByte((Convert.ToUInt32((电压不均衡3.Text) )) % 256);
            sendobj4.Data[4] = Convert.ToByte("00");
            sendobj4.Data[5] = Convert.ToByte("00");
            sendobj4.Data[6] = Convert.ToByte("00");
            sendobj4.Data[7] = Convert.ToByte("00");
            if (VCI_Transmit(m_devtype, m_devind, m_canind, ref sendobj4, 1) == 0)
            {
                MessageBox.Show("发送失败", "错误",
                        MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
            System.Threading.Thread.Sleep(500);
            if (m_bOpen == 0)
                return;
            VCI_CAN_OBJ sendobj5 = new VCI_CAN_OBJ();
            sendobj5.ExternFlag = 1;
            sendobj5.ID = System.Convert.ToUInt32("18000029", 16);
            sendobj5.DataLen = 8;
            sendobj5.Data[0] = Convert.ToByte("127");
            sendobj5.Data[1] = Convert.ToByte("110");
            sendobj5.Data[2] = Convert.ToByte("00");
            sendobj5.Data[3] = Convert.ToByte((Convert.ToUInt32((电压不均衡3.Text) )) / 256);
            sendobj5.Data[4] = Convert.ToByte("00");
            sendobj5.Data[5] = Convert.ToByte("00");
            sendobj5.Data[6] = Convert.ToByte("00");
            sendobj5.Data[7] = Convert.ToByte("00");
            if (VCI_Transmit(m_devtype, m_devind, m_canind, ref sendobj5, 1) == 0)
            {
                MessageBox.Show("发送失败", "错误",
                        MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
            System.Threading.Thread.Sleep(500);
            VCI_CAN_OBJ sendobj6 = new VCI_CAN_OBJ();
            sendobj6.ExternFlag = 1;
            sendobj6.ID = System.Convert.ToUInt32("18000029", 16);
            sendobj6.DataLen = 8;
            sendobj6.Data[0] = Convert.ToByte("191");
            sendobj6.Data[1] = Convert.ToByte("105");
            sendobj6.Data[2] = Convert.ToByte("00");
            sendobj6.Data[3] = Convert.ToByte("00");
            sendobj6.Data[4] = Convert.ToByte("00");
            sendobj6.Data[5] = Convert.ToByte("00");
            sendobj6.Data[6] = Convert.ToByte("00");
            sendobj6.Data[7] = Convert.ToByte("00");
            if (VCI_Transmit(m_devtype, m_devind, m_canind, ref sendobj6, 1) == 0)
            {
                MessageBox.Show("发送失败", "错误",
                        MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
            System.Threading.Thread.Sleep(1000);
            VCI_CAN_OBJ sendobj7 = new VCI_CAN_OBJ();
            sendobj7.ExternFlag = 1;
            sendobj7.ID = System.Convert.ToUInt32("18000029", 16);
            sendobj7.DataLen = 8;
            sendobj7.Data[0] = Convert.ToByte("191");
            sendobj7.Data[1] = Convert.ToByte("106");
            sendobj7.Data[2] = Convert.ToByte("00");
            sendobj7.Data[3] = Convert.ToByte("00");
            sendobj7.Data[4] = Convert.ToByte("00");
            sendobj7.Data[5] = Convert.ToByte("00");
            sendobj7.Data[6] = Convert.ToByte("00");
            sendobj7.Data[7] = Convert.ToByte("00");
            if (VCI_Transmit(m_devtype, m_devind, m_canind, ref sendobj7, 1) == 0)
            {
                MessageBox.Show("发送失败", "错误",
                        MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
            System.Threading.Thread.Sleep(1000);
            VCI_CAN_OBJ sendobj8 = new VCI_CAN_OBJ();
            sendobj8.ExternFlag = 1;
            sendobj8.ID = System.Convert.ToUInt32("18000029", 16);
            sendobj8.DataLen = 8;
            sendobj8.Data[0] = Convert.ToByte("191");
            sendobj8.Data[1] = Convert.ToByte("107");
            sendobj8.Data[2] = Convert.ToByte("00");
            sendobj8.Data[3] = Convert.ToByte("00");
            sendobj8.Data[4] = Convert.ToByte("00");
            sendobj8.Data[5] = Convert.ToByte("00");
            sendobj8.Data[6] = Convert.ToByte("00");
            sendobj8.Data[7] = Convert.ToByte("00");
            if (VCI_Transmit(m_devtype, m_devind, m_canind, ref sendobj8, 1) == 0)
            {
                MessageBox.Show("发送失败", "错误",
                        MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
            System.Threading.Thread.Sleep(1000);
            VCI_CAN_OBJ sendobj9 = new VCI_CAN_OBJ();
            sendobj9.ExternFlag = 1;
            sendobj9.ID = System.Convert.ToUInt32("18000029", 16);
            sendobj9.DataLen = 8;
            sendobj9.Data[0] = Convert.ToByte("191");
            sendobj9.Data[1] = Convert.ToByte("108");
            sendobj9.Data[2] = Convert.ToByte("00");
            sendobj9.Data[3] = Convert.ToByte("00");
            sendobj9.Data[4] = Convert.ToByte("00");
            sendobj9.Data[5] = Convert.ToByte("00");
            sendobj9.Data[6] = Convert.ToByte("00");
            sendobj9.Data[7] = Convert.ToByte("00");
            if (VCI_Transmit(m_devtype, m_devind, m_canind, ref sendobj9, 1) == 0)
            {
                MessageBox.Show("发送失败", "错误",
                        MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
            System.Threading.Thread.Sleep(1000);
            VCI_CAN_OBJ sendobj10 = new VCI_CAN_OBJ();
            sendobj10.ExternFlag = 1;
            sendobj10.ID = System.Convert.ToUInt32("18000029", 16);
            sendobj10.DataLen = 8;
            sendobj10.Data[0] = Convert.ToByte("191");
            sendobj10.Data[1] = Convert.ToByte("109");
            sendobj10.Data[2] = Convert.ToByte("00");
            sendobj10.Data[3] = Convert.ToByte("00");
            sendobj10.Data[4] = Convert.ToByte("00");
            sendobj10.Data[5] = Convert.ToByte("00");
            sendobj10.Data[6] = Convert.ToByte("00");
            sendobj10.Data[7] = Convert.ToByte("00");
            if (VCI_Transmit(m_devtype, m_devind, m_canind, ref sendobj10, 1) == 0)
            {
                MessageBox.Show("发送失败", "错误",
                        MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
            System.Threading.Thread.Sleep(1000);
            VCI_CAN_OBJ sendobj11 = new VCI_CAN_OBJ();
            sendobj11.ExternFlag = 1;
            sendobj11.ID = System.Convert.ToUInt32("18000029", 16);
            sendobj11.DataLen = 8;
            sendobj11.Data[0] = Convert.ToByte("191");
            sendobj11.Data[1] = Convert.ToByte("110");
            sendobj11.Data[2] = Convert.ToByte("00");
            sendobj11.Data[3] = Convert.ToByte("00");
            sendobj11.Data[4] = Convert.ToByte("00");
            sendobj11.Data[5] = Convert.ToByte("00");
            sendobj11.Data[6] = Convert.ToByte("00");
            sendobj11.Data[7] = Convert.ToByte("00");
            if (VCI_Transmit(m_devtype, m_devind, m_canind, ref sendobj11, 1) == 0)
            {
                MessageBox.Show("发送失败", "错误",
                        MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
            System.Threading.Thread.Sleep(1500);
            callback1.ID27 = temp[29]; callback1.ID28 = temp[30]; callback1.ID29 = temp[31];
            电压不均衡标定.Enabled = true;
        }
        unsafe private void 温度不均衡标定_Click(object sender, EventArgs e)
        {
            温度不均衡标定.Enabled = false;
            if (m_bOpen == 0)
                return;
            VCI_CAN_OBJ sendobj = new VCI_CAN_OBJ();
            sendobj.ExternFlag = 1;
            sendobj.ID = System.Convert.ToUInt32("18000029", 16);
            sendobj.DataLen = 8;
            sendobj.Data[0] = Convert.ToByte("127");
            sendobj.Data[1] = Convert.ToByte("111");
            sendobj.Data[2] = Convert.ToByte("00");
            sendobj.Data[3] = Convert.ToByte((Convert.ToInt64(温度不均衡1.Text)));
            sendobj.Data[4] = Convert.ToByte("00");
            sendobj.Data[5] = Convert.ToByte("00");
            sendobj.Data[6] = Convert.ToByte("00");
            sendobj.Data[7] = Convert.ToByte("00");
            if (VCI_Transmit(m_devtype, m_devind, m_canind, ref sendobj, 1) == 0)
            {
                MessageBox.Show("发送失败", "错误",
                        MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
            System.Threading.Thread.Sleep(500);
            if (m_bOpen == 0)
                return;
            VCI_CAN_OBJ sendobj1 = new VCI_CAN_OBJ();
            sendobj1.ExternFlag = 1;
            sendobj1.ID = System.Convert.ToUInt32("18000029", 16);
            sendobj1.DataLen = 8;
            sendobj1.Data[0] = Convert.ToByte("127");
            sendobj1.Data[1] = Convert.ToByte("112");
            sendobj1.Data[2] = Convert.ToByte("00");
            sendobj1.Data[3] = Convert.ToByte((Convert.ToInt64(温度不均衡2.Text)));
            sendobj1.Data[4] = Convert.ToByte("00");
            sendobj1.Data[5] = Convert.ToByte("00");
            sendobj1.Data[6] = Convert.ToByte("00");
            sendobj1.Data[7] = Convert.ToByte("00");
            if (VCI_Transmit(m_devtype, m_devind, m_canind, ref sendobj1, 1) == 0)
            {
                MessageBox.Show("发送失败", "错误",
                        MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
            System.Threading.Thread.Sleep(500);
            if (m_bOpen == 0)
                return;
            VCI_CAN_OBJ sendobj2 = new VCI_CAN_OBJ();
            sendobj2.ExternFlag = 1;
            sendobj2.ID = System.Convert.ToUInt32("18000029", 16);
            sendobj2.DataLen = 8;
            sendobj2.Data[0] = Convert.ToByte("127");
            sendobj2.Data[1] = Convert.ToByte("113");
            sendobj2.Data[2] = Convert.ToByte("00");
            sendobj2.Data[3] = Convert.ToByte((Convert.ToInt64(温度不均衡3.Text)) );
            sendobj2.Data[4] = Convert.ToByte("00");
            sendobj2.Data[5] = Convert.ToByte("00");
            sendobj2.Data[6] = Convert.ToByte("00");
            sendobj2.Data[7] = Convert.ToByte("00");
            if (VCI_Transmit(m_devtype, m_devind, m_canind, ref sendobj2, 1) == 0)
            {
                MessageBox.Show("发送失败", "错误",
                        MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
            System.Threading.Thread.Sleep(500);
            VCI_CAN_OBJ sendobj6 = new VCI_CAN_OBJ();
            sendobj6.ExternFlag = 1;
            sendobj6.ID = System.Convert.ToUInt32("18000029", 16);
            sendobj6.DataLen = 8;
            sendobj6.Data[0] = Convert.ToByte("191");
            sendobj6.Data[1] = Convert.ToByte("111");
            sendobj6.Data[2] = Convert.ToByte("00");
            sendobj6.Data[3] = Convert.ToByte("00");
            sendobj6.Data[4] = Convert.ToByte("00");
            sendobj6.Data[5] = Convert.ToByte("00");
            sendobj6.Data[6] = Convert.ToByte("00");
            sendobj6.Data[7] = Convert.ToByte("00");
            if (VCI_Transmit(m_devtype, m_devind, m_canind, ref sendobj6, 1) == 0)
            {
                MessageBox.Show("发送失败", "错误",
                        MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
            System.Threading.Thread.Sleep(1000);
            VCI_CAN_OBJ sendobj7 = new VCI_CAN_OBJ();
            sendobj7.ExternFlag = 1;
            sendobj7.ID = System.Convert.ToUInt32("18000029", 16);
            sendobj7.DataLen = 8;
            sendobj7.Data[0] = Convert.ToByte("191");
            sendobj7.Data[1] = Convert.ToByte("112");
            sendobj7.Data[2] = Convert.ToByte("00");
            sendobj7.Data[3] = Convert.ToByte("00");
            sendobj7.Data[4] = Convert.ToByte("00");
            sendobj7.Data[5] = Convert.ToByte("00");
            sendobj7.Data[6] = Convert.ToByte("00");
            sendobj7.Data[7] = Convert.ToByte("00");
            if (VCI_Transmit(m_devtype, m_devind, m_canind, ref sendobj7, 1) == 0)
            {
                MessageBox.Show("发送失败", "错误",
                        MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
            System.Threading.Thread.Sleep(1000);
            VCI_CAN_OBJ sendobj8 = new VCI_CAN_OBJ();
            sendobj8.ExternFlag = 1;
            sendobj8.ID = System.Convert.ToUInt32("18000029", 16);
            sendobj8.DataLen = 8;
            sendobj8.Data[0] = Convert.ToByte("191");
            sendobj8.Data[1] = Convert.ToByte("113");
            sendobj8.Data[2] = Convert.ToByte("00");
            sendobj8.Data[3] = Convert.ToByte("00");
            sendobj8.Data[4] = Convert.ToByte("00");
            sendobj8.Data[5] = Convert.ToByte("00");
            sendobj8.Data[6] = Convert.ToByte("00");
            sendobj8.Data[7] = Convert.ToByte("00");
            if (VCI_Transmit(m_devtype, m_devind, m_canind, ref sendobj8, 1) == 0)
            {
                MessageBox.Show("发送失败", "错误",
                        MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
            System.Threading.Thread.Sleep(1500);
            callback1.ID30 = temp[32]; callback1.ID31 = temp[33]; callback1.ID32 = temp[34];
            温度不均衡标定.Enabled = true;

        }
        unsafe private void SOC过低标定_Click(object sender, EventArgs e)
        {
            SOC过低标定.Enabled = false;
            if (m_bOpen == 0)
                return;
            VCI_CAN_OBJ sendobj = new VCI_CAN_OBJ();
            sendobj.ExternFlag = 1;
            sendobj.ID = System.Convert.ToUInt32("18000029", 16);
            sendobj.DataLen = 8;
            sendobj.Data[0] = Convert.ToByte("127");
            sendobj.Data[1] = Convert.ToByte("114");
            sendobj.Data[2] = Convert.ToByte("00");
            sendobj.Data[3] = Convert.ToByte((Convert.ToUInt32(SOC过低1.Text)*2.5));
            sendobj.Data[4] = Convert.ToByte("00");
            sendobj.Data[5] = Convert.ToByte("00");
            sendobj.Data[6] = Convert.ToByte("00");
            sendobj.Data[7] = Convert.ToByte("00");
            if (VCI_Transmit(m_devtype, m_devind, m_canind, ref sendobj, 1) == 0)
            {
                MessageBox.Show("发送失败", "错误",
                        MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
            System.Threading.Thread.Sleep(500);
            if (m_bOpen == 0)
                return;
            VCI_CAN_OBJ sendobj1 = new VCI_CAN_OBJ();
            sendobj1.ExternFlag = 1;
            sendobj1.ID = System.Convert.ToUInt32("18000029", 16);
            sendobj1.DataLen = 8;
            sendobj1.Data[0] = Convert.ToByte("127");
            sendobj1.Data[1] = Convert.ToByte("115");
            sendobj1.Data[2] = Convert.ToByte("00");
            sendobj1.Data[3] = Convert.ToByte((Convert.ToUInt32(SOC过低2.Text)*2.5));
            sendobj1.Data[4] = Convert.ToByte("00");
            sendobj1.Data[5] = Convert.ToByte("00");
            sendobj1.Data[6] = Convert.ToByte("00");
            sendobj1.Data[7] = Convert.ToByte("00");
            if (VCI_Transmit(m_devtype, m_devind, m_canind, ref sendobj1, 1) == 0)
            {
                MessageBox.Show("发送失败", "错误",
                        MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
            System.Threading.Thread.Sleep(500);
            if (m_bOpen == 0)
                return;
            VCI_CAN_OBJ sendobj2 = new VCI_CAN_OBJ();
            sendobj2.ExternFlag = 1;
            sendobj2.ID = System.Convert.ToUInt32("18000029", 16);
            sendobj2.DataLen = 8;
            sendobj2.Data[0] = Convert.ToByte("127");
            sendobj2.Data[1] = Convert.ToByte("116");
            sendobj2.Data[2] = Convert.ToByte("00");
            sendobj2.Data[3] = Convert.ToByte((Convert.ToUInt32(SOC过低3.Text)*2.5));
            sendobj2.Data[4] = Convert.ToByte("00");
            sendobj2.Data[5] = Convert.ToByte("00");
            sendobj2.Data[6] = Convert.ToByte("00");
            sendobj2.Data[7] = Convert.ToByte("00");
            if (VCI_Transmit(m_devtype, m_devind, m_canind, ref sendobj2, 1) == 0)
            {
                MessageBox.Show("发送失败", "错误",
                        MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
            System.Threading.Thread.Sleep(500);
            VCI_CAN_OBJ sendobj6 = new VCI_CAN_OBJ();
            sendobj6.ExternFlag = 1;
            sendobj6.ID = System.Convert.ToUInt32("18000029", 16);
            sendobj6.DataLen = 8;
            sendobj6.Data[0] = Convert.ToByte("191");
            sendobj6.Data[1] = Convert.ToByte("114");
            sendobj6.Data[2] = Convert.ToByte("00");
            sendobj6.Data[3] = Convert.ToByte("00");
            sendobj6.Data[4] = Convert.ToByte("00");
            sendobj6.Data[5] = Convert.ToByte("00");
            sendobj6.Data[6] = Convert.ToByte("00");
            sendobj6.Data[7] = Convert.ToByte("00");
            if (VCI_Transmit(m_devtype, m_devind, m_canind, ref sendobj6, 1) == 0)
            {
                MessageBox.Show("发送失败", "错误",
                        MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
            System.Threading.Thread.Sleep(1000);
            VCI_CAN_OBJ sendobj7 = new VCI_CAN_OBJ();
            sendobj7.ExternFlag = 1;
            sendobj7.ID = System.Convert.ToUInt32("18000029", 16);
            sendobj7.DataLen = 8;
            sendobj7.Data[0] = Convert.ToByte("191");
            sendobj7.Data[1] = Convert.ToByte("115");
            sendobj7.Data[2] = Convert.ToByte("00");
            sendobj7.Data[3] = Convert.ToByte("00");
            sendobj7.Data[4] = Convert.ToByte("00");
            sendobj7.Data[5] = Convert.ToByte("00");
            sendobj7.Data[6] = Convert.ToByte("00");
            sendobj7.Data[7] = Convert.ToByte("00");
            if (VCI_Transmit(m_devtype, m_devind, m_canind, ref sendobj7, 1) == 0)
            {
                MessageBox.Show("发送失败", "错误",
                        MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
            System.Threading.Thread.Sleep(1000);
            VCI_CAN_OBJ sendobj8 = new VCI_CAN_OBJ();
            sendobj8.ExternFlag = 1;
            sendobj8.ID = System.Convert.ToUInt32("18000029", 16);
            sendobj8.DataLen = 8;
            sendobj8.Data[0] = Convert.ToByte("191");
            sendobj8.Data[1] = Convert.ToByte("116");
            sendobj8.Data[2] = Convert.ToByte("00");
            sendobj8.Data[3] = Convert.ToByte("00");
            sendobj8.Data[4] = Convert.ToByte("00");
            sendobj8.Data[5] = Convert.ToByte("00");
            sendobj8.Data[6] = Convert.ToByte("00");
            sendobj8.Data[7] = Convert.ToByte("00");
            if (VCI_Transmit(m_devtype, m_devind, m_canind, ref sendobj8, 1) == 0)
            {
                MessageBox.Show("发送失败", "错误",
                        MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
            System.Threading.Thread.Sleep(1500);
            callback1.ID33 = temp[35]; callback1.ID34 = temp[36]; callback1.ID35 = temp[37];
            SOC过低标定.Enabled = true;
        }
        unsafe private void MOS温度过高标定_Click(object sender, EventArgs e)
        {
            MOS温度过高标定.Enabled = false;
            if (m_bOpen == 0)
                return;
            VCI_CAN_OBJ sendobj = new VCI_CAN_OBJ();
            sendobj.ExternFlag = 1;
            sendobj.ID = System.Convert.ToUInt32("18000029", 16);
            sendobj.DataLen = 8;
            sendobj.Data[0] = Convert.ToByte("127");
            sendobj.Data[1] = Convert.ToByte("238");
            sendobj.Data[2] = Convert.ToByte("00");
            sendobj.Data[3] = Convert.ToByte((Convert.ToUInt32(MOS温度过高1.Text)) + 40);
            sendobj.Data[4] = Convert.ToByte("00");
            sendobj.Data[5] = Convert.ToByte("00");
            sendobj.Data[6] = Convert.ToByte("00");
            sendobj.Data[7] = Convert.ToByte("00");
            if (VCI_Transmit(m_devtype, m_devind, m_canind, ref sendobj, 1) == 0)
            {
                MessageBox.Show("发送失败", "错误",
                        MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
            System.Threading.Thread.Sleep(100);
            if (m_bOpen == 0)
                return;
            VCI_CAN_OBJ sendobj1 = new VCI_CAN_OBJ();
            sendobj1.ExternFlag = 1;
            sendobj1.ID = System.Convert.ToUInt32("18000029", 16);
            sendobj1.DataLen = 8;
            sendobj1.Data[0] = Convert.ToByte("127");
            sendobj1.Data[1] = Convert.ToByte("239");
            sendobj1.Data[2] = Convert.ToByte("00");
            sendobj1.Data[3] = Convert.ToByte((Convert.ToUInt32(MOS温度过高2.Text)) + 40);
            sendobj1.Data[4] = Convert.ToByte("00");
            sendobj1.Data[5] = Convert.ToByte("00");
            sendobj1.Data[6] = Convert.ToByte("00");
            sendobj1.Data[7] = Convert.ToByte("00");
            if (VCI_Transmit(m_devtype, m_devind, m_canind, ref sendobj1, 1) == 0)
            {
                MessageBox.Show("发送失败", "错误",
                        MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
            System.Threading.Thread.Sleep(100);
            if (m_bOpen == 0)
                return;
            VCI_CAN_OBJ sendobj2 = new VCI_CAN_OBJ();
            sendobj2.ExternFlag = 1;
            sendobj2.ID = System.Convert.ToUInt32("18000029", 16);
            sendobj2.DataLen = 8;
            sendobj2.Data[0] = Convert.ToByte("127");
            sendobj2.Data[1] = Convert.ToByte("240");
            sendobj2.Data[2] = Convert.ToByte("00");
            sendobj2.Data[3] = Convert.ToByte((Convert.ToUInt32(MOS温度过高3.Text)) + 40);
            sendobj2.Data[4] = Convert.ToByte("00");
            sendobj2.Data[5] = Convert.ToByte("00");
            sendobj2.Data[6] = Convert.ToByte("00");
            sendobj2.Data[7] = Convert.ToByte("00");
            if (VCI_Transmit(m_devtype, m_devind, m_canind, ref sendobj2, 1) == 0)
            {
                MessageBox.Show("发送失败", "错误",
                        MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
            System.Threading.Thread.Sleep(100);
            VCI_CAN_OBJ sendobj6 = new VCI_CAN_OBJ();
            sendobj6.ExternFlag = 1;
            sendobj6.ID = System.Convert.ToUInt32("18000029", 16);
            sendobj6.DataLen = 8;
            sendobj6.Data[0] = Convert.ToByte("191");
            sendobj6.Data[1] = Convert.ToByte("238");
            sendobj6.Data[2] = Convert.ToByte("00");
            sendobj6.Data[3] = Convert.ToByte("00");
            sendobj6.Data[4] = Convert.ToByte("00");
            sendobj6.Data[5] = Convert.ToByte("00");
            sendobj6.Data[6] = Convert.ToByte("00");
            sendobj6.Data[7] = Convert.ToByte("00");
            if (VCI_Transmit(m_devtype, m_devind, m_canind, ref sendobj6, 1) == 0)
            {
                MessageBox.Show("发送失败", "错误",
                        MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
            System.Threading.Thread.Sleep(1000);
            VCI_CAN_OBJ sendobj7 = new VCI_CAN_OBJ();
            sendobj7.ExternFlag = 1;
            sendobj7.ID = System.Convert.ToUInt32("18000029", 16);
            sendobj7.DataLen = 8;
            sendobj7.Data[0] = Convert.ToByte("191");
            sendobj7.Data[1] = Convert.ToByte("239");
            sendobj7.Data[2] = Convert.ToByte("00");
            sendobj7.Data[3] = Convert.ToByte("00");
            sendobj7.Data[4] = Convert.ToByte("00");
            sendobj7.Data[5] = Convert.ToByte("00");
            sendobj7.Data[6] = Convert.ToByte("00");
            sendobj7.Data[7] = Convert.ToByte("00");
            if (VCI_Transmit(m_devtype, m_devind, m_canind, ref sendobj7, 1) == 0)
            {
                MessageBox.Show("发送失败", "错误",
                        MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
            System.Threading.Thread.Sleep(1000);
            VCI_CAN_OBJ sendobj8 = new VCI_CAN_OBJ();
            sendobj8.ExternFlag = 1;
            sendobj8.ID = System.Convert.ToUInt32("18000029", 16);
            sendobj8.DataLen = 8;
            sendobj8.Data[0] = Convert.ToByte("191");
            sendobj8.Data[1] = Convert.ToByte("240");
            sendobj8.Data[2] = Convert.ToByte("00");
            sendobj8.Data[3] = Convert.ToByte("00");
            sendobj8.Data[4] = Convert.ToByte("00");
            sendobj8.Data[5] = Convert.ToByte("00");
            sendobj8.Data[6] = Convert.ToByte("00");
            sendobj8.Data[7] = Convert.ToByte("00");
            if (VCI_Transmit(m_devtype, m_devind, m_canind, ref sendobj8, 1) == 0)
            {
                MessageBox.Show("发送失败", "错误",
                        MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
            System.Threading.Thread.Sleep(1500);
            callback1.ID36 = temp[38]; callback1.ID37 = temp[39]; callback1.ID38 = temp[40];
            MOS温度过高标定.Enabled = true;

        }
       unsafe private void 充电低温标定_Click(object sender, EventArgs e)
        {
            充电低温标定.Enabled = false;
            if (m_bOpen == 0)
                return;
            VCI_CAN_OBJ sendobj = new VCI_CAN_OBJ();
            sendobj.ExternFlag = 1;
            sendobj.ID = System.Convert.ToUInt32("18000029", 16);
            sendobj.DataLen = 8;
            sendobj.Data[0] = Convert.ToByte("127");
            sendobj.Data[1] = Convert.ToByte("22");
            sendobj.Data[2] = Convert.ToByte("00");
            sendobj.Data[3] = Convert.ToByte((Convert.ToInt64(充电低温1.Text)) + 40);
            sendobj.Data[4] = Convert.ToByte("00");
            sendobj.Data[5] = Convert.ToByte("00");
            sendobj.Data[6] = Convert.ToByte("00");
            sendobj.Data[7] = Convert.ToByte("00");
            if (VCI_Transmit(m_devtype, m_devind, m_canind, ref sendobj, 1) == 0)
            {
                MessageBox.Show("发送失败", "错误",
                        MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
            System.Threading.Thread.Sleep(100);
            if (m_bOpen == 0)
                return;
            VCI_CAN_OBJ sendobj1 = new VCI_CAN_OBJ();
            sendobj1.ExternFlag = 1;
            sendobj1.ID = System.Convert.ToUInt32("18000029", 16);
            sendobj1.DataLen = 8;
            sendobj1.Data[0] = Convert.ToByte("127");
            sendobj1.Data[1] = Convert.ToByte("23");
            sendobj1.Data[2] = Convert.ToByte("00");
            sendobj1.Data[3] = Convert.ToByte((Convert.ToInt64(充电低温2.Text)) + 40);
            sendobj1.Data[4] = Convert.ToByte("00");
            sendobj1.Data[5] = Convert.ToByte("00");
            sendobj1.Data[6] = Convert.ToByte("00");
            sendobj1.Data[7] = Convert.ToByte("00");
            if (VCI_Transmit(m_devtype, m_devind, m_canind, ref sendobj1, 1) == 0)
            {
                MessageBox.Show("发送失败", "错误",
                        MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
            System.Threading.Thread.Sleep(100);
            if (m_bOpen == 0)
                return;
            VCI_CAN_OBJ sendobj2 = new VCI_CAN_OBJ();
            sendobj2.ExternFlag = 1;
            sendobj2.ID = System.Convert.ToUInt32("18000029", 16);
            sendobj2.DataLen = 8;
            sendobj2.Data[0] = Convert.ToByte("127");
            sendobj2.Data[1] = Convert.ToByte("24");
            sendobj2.Data[2] = Convert.ToByte("00");
            sendobj2.Data[3] = Convert.ToByte((Convert.ToInt64(充电低温3.Text)) + 40);
            sendobj2.Data[4] = Convert.ToByte("00");
            sendobj2.Data[5] = Convert.ToByte("00");
            sendobj2.Data[6] = Convert.ToByte("00");
            sendobj2.Data[7] = Convert.ToByte("00");
            if (VCI_Transmit(m_devtype, m_devind, m_canind, ref sendobj2, 1) == 0)
            {
                MessageBox.Show("发送失败", "错误",
                        MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
            System.Threading.Thread.Sleep(100);
            VCI_CAN_OBJ sendobj6 = new VCI_CAN_OBJ();
            sendobj6.ExternFlag = 1;
            sendobj6.ID = System.Convert.ToUInt32("18000029", 16);
            sendobj6.DataLen = 8;
            sendobj6.Data[0] = Convert.ToByte("191");
            sendobj6.Data[1] = Convert.ToByte("22");
            sendobj6.Data[2] = Convert.ToByte("00");
            sendobj6.Data[3] = Convert.ToByte("00");
            sendobj6.Data[4] = Convert.ToByte("00");
            sendobj6.Data[5] = Convert.ToByte("00");
            sendobj6.Data[6] = Convert.ToByte("00");
            sendobj6.Data[7] = Convert.ToByte("00");
            if (VCI_Transmit(m_devtype, m_devind, m_canind, ref sendobj6, 1) == 0)
            {
                MessageBox.Show("发送失败", "错误",
                        MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
            System.Threading.Thread.Sleep(1000);
            VCI_CAN_OBJ sendobj7 = new VCI_CAN_OBJ();
            sendobj7.ExternFlag = 1;
            sendobj7.ID = System.Convert.ToUInt32("18000029", 16);
            sendobj7.DataLen = 8;
            sendobj7.Data[0] = Convert.ToByte("191");
            sendobj7.Data[1] = Convert.ToByte("23");
            sendobj7.Data[2] = Convert.ToByte("00");
            sendobj7.Data[3] = Convert.ToByte("00");
            sendobj7.Data[4] = Convert.ToByte("00");
            sendobj7.Data[5] = Convert.ToByte("00");
            sendobj7.Data[6] = Convert.ToByte("00");
            sendobj7.Data[7] = Convert.ToByte("00");
            if (VCI_Transmit(m_devtype, m_devind, m_canind, ref sendobj7, 1) == 0)
            {
                MessageBox.Show("发送失败", "错误",
                        MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
            System.Threading.Thread.Sleep(1000);
            VCI_CAN_OBJ sendobj8 = new VCI_CAN_OBJ();
            sendobj8.ExternFlag = 1;
            sendobj8.ID = System.Convert.ToUInt32("18000029", 16);
            sendobj8.DataLen = 8;
            sendobj8.Data[0] = Convert.ToByte("191");
            sendobj8.Data[1] = Convert.ToByte("24");
            sendobj8.Data[2] = Convert.ToByte("00");
            sendobj8.Data[3] = Convert.ToByte("00");
            sendobj8.Data[4] = Convert.ToByte("00");
            sendobj8.Data[5] = Convert.ToByte("00");
            sendobj8.Data[6] = Convert.ToByte("00");
            sendobj8.Data[7] = Convert.ToByte("00");
            if (VCI_Transmit(m_devtype, m_devind, m_canind, ref sendobj8, 1) == 0)
            {
                MessageBox.Show("发送失败", "错误",
                        MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
            System.Threading.Thread.Sleep(1500);
            callback1.ID39 = temp[41]; callback1.ID40 = temp[42]; callback1.ID41 = temp[43];
            充电低温标定.Enabled = true;

        }
        unsafe private void 充电高温标定_Click(object sender, EventArgs e)
        {
            充电高温标定.Enabled = false;
            if (m_bOpen == 0)
                return;
            VCI_CAN_OBJ sendobj = new VCI_CAN_OBJ();
            sendobj.ExternFlag = 1;
            sendobj.ID = System.Convert.ToUInt32("18000029", 16);
            sendobj.DataLen = 8;
            sendobj.Data[0] = Convert.ToByte("127");
            sendobj.Data[1] = Convert.ToByte("241");
            sendobj.Data[2] = Convert.ToByte("00");
            sendobj.Data[3] = Convert.ToByte((Convert.ToInt64(充电高温1.Text)) + 40);
            sendobj.Data[4] = Convert.ToByte("00");
            sendobj.Data[5] = Convert.ToByte("00");
            sendobj.Data[6] = Convert.ToByte("00");
            sendobj.Data[7] = Convert.ToByte("00");
            if (VCI_Transmit(m_devtype, m_devind, m_canind, ref sendobj, 1) == 0)
            {
                MessageBox.Show("发送失败", "错误",
                        MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
            System.Threading.Thread.Sleep(100);
            if (m_bOpen == 0)
                return;
            VCI_CAN_OBJ sendobj1 = new VCI_CAN_OBJ();
            sendobj1.ExternFlag = 1;
            sendobj1.ID = System.Convert.ToUInt32("18000029", 16);
            sendobj1.DataLen = 8;
            sendobj1.Data[0] = Convert.ToByte("127");
            sendobj1.Data[1] = Convert.ToByte("242");
            sendobj1.Data[2] = Convert.ToByte("00");
            sendobj1.Data[3] = Convert.ToByte((Convert.ToInt64(充电高温2.Text)) + 40);
            sendobj1.Data[4] = Convert.ToByte("00");
            sendobj1.Data[5] = Convert.ToByte("00");
            sendobj1.Data[6] = Convert.ToByte("00");
            sendobj1.Data[7] = Convert.ToByte("00");
            if (VCI_Transmit(m_devtype, m_devind, m_canind, ref sendobj1, 1) == 0)
            {
                MessageBox.Show("发送失败", "错误",
                        MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
            System.Threading.Thread.Sleep(100);
            if (m_bOpen == 0)
                return;
            VCI_CAN_OBJ sendobj2 = new VCI_CAN_OBJ();
            sendobj2.ExternFlag = 1;
            sendobj2.ID = System.Convert.ToUInt32("18000029", 16);
            sendobj2.DataLen = 8;
            sendobj2.Data[0] = Convert.ToByte("127");
            sendobj2.Data[1] = Convert.ToByte("243");
            sendobj2.Data[2] = Convert.ToByte("00");
            sendobj2.Data[3] = Convert.ToByte((Convert.ToInt64(充电高温3.Text)) + 40);
            sendobj2.Data[4] = Convert.ToByte("00");
            sendobj2.Data[5] = Convert.ToByte("00");
            sendobj2.Data[6] = Convert.ToByte("00");
            sendobj2.Data[7] = Convert.ToByte("00");
            if (VCI_Transmit(m_devtype, m_devind, m_canind, ref sendobj2, 1) == 0)
            {
                MessageBox.Show("发送失败", "错误",
                        MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
            System.Threading.Thread.Sleep(100);
            VCI_CAN_OBJ sendobj6 = new VCI_CAN_OBJ();
            sendobj6.ExternFlag = 1;
            sendobj6.ID = System.Convert.ToUInt32("18000029", 16);
            sendobj6.DataLen = 8;
            sendobj6.Data[0] = Convert.ToByte("191");
            sendobj6.Data[1] = Convert.ToByte("241");
            sendobj6.Data[2] = Convert.ToByte("00");
            sendobj6.Data[3] = Convert.ToByte("00");
            sendobj6.Data[4] = Convert.ToByte("00");
            sendobj6.Data[5] = Convert.ToByte("00");
            sendobj6.Data[6] = Convert.ToByte("00");
            sendobj6.Data[7] = Convert.ToByte("00");
            if (VCI_Transmit(m_devtype, m_devind, m_canind, ref sendobj6, 1) == 0)
            {
                MessageBox.Show("发送失败", "错误",
                        MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
            System.Threading.Thread.Sleep(1000);
            VCI_CAN_OBJ sendobj7 = new VCI_CAN_OBJ();
            sendobj7.ExternFlag = 1;
            sendobj7.ID = System.Convert.ToUInt32("18000029", 16);
            sendobj7.DataLen = 8;
            sendobj7.Data[0] = Convert.ToByte("191");
            sendobj7.Data[1] = Convert.ToByte("242");
            sendobj7.Data[2] = Convert.ToByte("00");
            sendobj7.Data[3] = Convert.ToByte("00");
            sendobj7.Data[4] = Convert.ToByte("00");
            sendobj7.Data[5] = Convert.ToByte("00");
            sendobj7.Data[6] = Convert.ToByte("00");
            sendobj7.Data[7] = Convert.ToByte("00");
            if (VCI_Transmit(m_devtype, m_devind, m_canind, ref sendobj7, 1) == 0)
            {
                MessageBox.Show("发送失败", "错误",
                        MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
            System.Threading.Thread.Sleep(1000);
            VCI_CAN_OBJ sendobj8 = new VCI_CAN_OBJ();
            sendobj8.ExternFlag = 1;
            sendobj8.ID = System.Convert.ToUInt32("18000029", 16);
            sendobj8.DataLen = 8;
            sendobj8.Data[0] = Convert.ToByte("191");
            sendobj8.Data[1] = Convert.ToByte("243");
            sendobj8.Data[2] = Convert.ToByte("00");
            sendobj8.Data[3] = Convert.ToByte("00");
            sendobj8.Data[4] = Convert.ToByte("00");
            sendobj8.Data[5] = Convert.ToByte("00");
            sendobj8.Data[6] = Convert.ToByte("00");
            sendobj8.Data[7] = Convert.ToByte("00");
            if (VCI_Transmit(m_devtype, m_devind, m_canind, ref sendobj8, 1) == 0)
            {
                MessageBox.Show("发送失败", "错误",
                        MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
            System.Threading.Thread.Sleep(1500);
            callback1.ID42 = temp[44]; callback1.ID43 = temp[45]; callback1.ID44 = temp[46];
            充电高温标定.Enabled = true;
        }
        unsafe private void 放电高温标定_Click(object sender, EventArgs e)
        {
            放电高温标定.Enabled = false;
            if (m_bOpen == 0)
                return;
            VCI_CAN_OBJ sendobj = new VCI_CAN_OBJ();
            sendobj.ExternFlag = 1;
            sendobj.ID = System.Convert.ToUInt32("18000029", 16);
            sendobj.DataLen = 8;
            sendobj.Data[0] = Convert.ToByte("127");
            sendobj.Data[1] = Convert.ToByte("244");
            sendobj.Data[2] = Convert.ToByte("00");
            sendobj.Data[3] = Convert.ToByte((Convert.ToInt64(放电高温1.Text)) + 40);
            sendobj.Data[4] = Convert.ToByte("00");
            sendobj.Data[5] = Convert.ToByte("00");
            sendobj.Data[6] = Convert.ToByte("00");
            sendobj.Data[7] = Convert.ToByte("00");
            if (VCI_Transmit(m_devtype, m_devind, m_canind, ref sendobj, 1) == 0)
            {
                MessageBox.Show("发送失败", "错误",
                        MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
            System.Threading.Thread.Sleep(100);
            if (m_bOpen == 0)
                return;
            VCI_CAN_OBJ sendobj1 = new VCI_CAN_OBJ();
            sendobj1.ExternFlag = 1;
            sendobj1.ID = System.Convert.ToUInt32("18000029", 16);
            sendobj1.DataLen = 8;
            sendobj1.Data[0] = Convert.ToByte("127");
            sendobj1.Data[1] = Convert.ToByte("245");
            sendobj1.Data[2] = Convert.ToByte("00");
            sendobj1.Data[3] = Convert.ToByte((Convert.ToInt64(放电高温2.Text)) + 40);
            sendobj1.Data[4] = Convert.ToByte("00");
            sendobj1.Data[5] = Convert.ToByte("00");
            sendobj1.Data[6] = Convert.ToByte("00");
            sendobj1.Data[7] = Convert.ToByte("00");
            if (VCI_Transmit(m_devtype, m_devind, m_canind, ref sendobj1, 1) == 0)
            {
                MessageBox.Show("发送失败", "错误",
                        MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
            System.Threading.Thread.Sleep(100);
            if (m_bOpen == 0)
                return;
            VCI_CAN_OBJ sendobj2 = new VCI_CAN_OBJ();
            sendobj2.ExternFlag = 1;
            sendobj2.ID = System.Convert.ToUInt32("18000029", 16);
            sendobj2.DataLen = 8;
            sendobj2.Data[0] = Convert.ToByte("127");
            sendobj2.Data[1] = Convert.ToByte("246");
            sendobj2.Data[2] = Convert.ToByte("00");
            sendobj2.Data[3] = Convert.ToByte((Convert.ToInt64(放电高温3.Text)) + 40);
            sendobj2.Data[4] = Convert.ToByte("00");
            sendobj2.Data[5] = Convert.ToByte("00");
            sendobj2.Data[6] = Convert.ToByte("00");
            sendobj2.Data[7] = Convert.ToByte("00");
            if (VCI_Transmit(m_devtype, m_devind, m_canind, ref sendobj2, 1) == 0)
            {
                MessageBox.Show("发送失败", "错误",
                        MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
            System.Threading.Thread.Sleep(100);
            VCI_CAN_OBJ sendobj6 = new VCI_CAN_OBJ();
            sendobj6.ExternFlag = 1;
            sendobj6.ID = System.Convert.ToUInt32("18000029", 16);
            sendobj6.DataLen = 8;
            sendobj6.Data[0] = Convert.ToByte("191");
            sendobj6.Data[1] = Convert.ToByte("244");
            sendobj6.Data[2] = Convert.ToByte("00");
            sendobj6.Data[3] = Convert.ToByte("00");
            sendobj6.Data[4] = Convert.ToByte("00");
            sendobj6.Data[5] = Convert.ToByte("00");
            sendobj6.Data[6] = Convert.ToByte("00");
            sendobj6.Data[7] = Convert.ToByte("00");
            if (VCI_Transmit(m_devtype, m_devind, m_canind, ref sendobj6, 1) == 0)
            {
                MessageBox.Show("发送失败", "错误",
                        MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
            System.Threading.Thread.Sleep(1000);
            VCI_CAN_OBJ sendobj7 = new VCI_CAN_OBJ();
            sendobj7.ExternFlag = 1;
            sendobj7.ID = System.Convert.ToUInt32("18000029", 16);
            sendobj7.DataLen = 8;
            sendobj7.Data[0] = Convert.ToByte("191");
            sendobj7.Data[1] = Convert.ToByte("245");
            sendobj7.Data[2] = Convert.ToByte("00");
            sendobj7.Data[3] = Convert.ToByte("00");
            sendobj7.Data[4] = Convert.ToByte("00");
            sendobj7.Data[5] = Convert.ToByte("00");
            sendobj7.Data[6] = Convert.ToByte("00");
            sendobj7.Data[7] = Convert.ToByte("00");
            if (VCI_Transmit(m_devtype, m_devind, m_canind, ref sendobj7, 1) == 0)
            {
                MessageBox.Show("发送失败", "错误",
                        MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
            System.Threading.Thread.Sleep(1000);
            VCI_CAN_OBJ sendobj8 = new VCI_CAN_OBJ();
            sendobj8.ExternFlag = 1;
            sendobj8.ID = System.Convert.ToUInt32("18000029", 16);
            sendobj8.DataLen = 8;
            sendobj8.Data[0] = Convert.ToByte("191");
            sendobj8.Data[1] = Convert.ToByte("246");
            sendobj8.Data[2] = Convert.ToByte("00");
            sendobj8.Data[3] = Convert.ToByte("00");
            sendobj8.Data[4] = Convert.ToByte("00");
            sendobj8.Data[5] = Convert.ToByte("00");
            sendobj8.Data[6] = Convert.ToByte("00");
            sendobj8.Data[7] = Convert.ToByte("00");
            if (VCI_Transmit(m_devtype, m_devind, m_canind, ref sendobj8, 1) == 0)
            {
                MessageBox.Show("发送失败", "错误",
                        MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
            System.Threading.Thread.Sleep(1500);
            callback1.ID45 = temp[47]; callback1.ID46 = temp[48]; callback1.ID47 = temp[49];
            放电高温标定.Enabled = true;
        }
        unsafe private void 轻微漏电时间标定_Click(object sender, EventArgs e)
        {
            轻微漏电时间标定.Enabled = false;
            UInt32[] a = new UInt32[4];
            if (m_bOpen == 0)
                return;
            VCI_CAN_OBJ sendobj = new VCI_CAN_OBJ();
            sendobj.ExternFlag = 1;
            sendobj.ID = System.Convert.ToUInt32("18000029", 16);
            sendobj.DataLen = 8;
            sendobj.Data[0] = Convert.ToByte("127");
            sendobj.Data[1] = Convert.ToByte("160");
            sendobj.Data[2] = Convert.ToByte("00");
            sendobj.Data[3] = Convert.ToByte((Convert.ToDouble(轻微漏电时间1.Text) ) * 10);
            sendobj.Data[4] = Convert.ToByte("00");
            sendobj.Data[5] = Convert.ToByte("00");
            sendobj.Data[6] = Convert.ToByte("00");
            sendobj.Data[7] = Convert.ToByte("00");
            if (VCI_Transmit(m_devtype, m_devind, m_canind, ref sendobj, 1) == 0)
            {
                MessageBox.Show("发送失败", "错误",
                        MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
            System.Threading.Thread.Sleep(100);

            VCI_CAN_OBJ sendobj1 = new VCI_CAN_OBJ();
            sendobj1.ExternFlag = 1;
            sendobj1.ID = System.Convert.ToUInt32("18000029", 16);
            sendobj1.DataLen = 8;
            sendobj1.Data[0] = Convert.ToByte("127");
            sendobj1.Data[1] = Convert.ToByte("161");
            sendobj1.Data[2] = Convert.ToByte("00");
            sendobj1.Data[3] = Convert.ToByte((Convert.ToDouble(轻微漏电时间2.Text) ) * 10);
            sendobj1.Data[4] = Convert.ToByte("00");
            sendobj1.Data[5] = Convert.ToByte("00");
            sendobj1.Data[6] = Convert.ToByte("00");
            sendobj1.Data[7] = Convert.ToByte("00");
            if (VCI_Transmit(m_devtype, m_devind, m_canind, ref sendobj1, 1) == 0)
            {
                MessageBox.Show("发送失败", "错误",
                        MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
            System.Threading.Thread.Sleep(100);
            if (m_bOpen == 0)
                return;
            VCI_CAN_OBJ sendobj2 = new VCI_CAN_OBJ();
            sendobj2.ExternFlag = 1;
            sendobj2.ID = System.Convert.ToUInt32("18000029", 16);
            sendobj2.DataLen = 8;
            sendobj2.Data[0] = Convert.ToByte("127");
            sendobj2.Data[1] = Convert.ToByte("162");
            sendobj2.Data[2] = Convert.ToByte("00");
            sendobj2.Data[3] = Convert.ToByte((Convert.ToDouble(轻微漏电时间3.Text))*10);
            sendobj2.Data[4] = Convert.ToByte("00");
            sendobj2.Data[5] = Convert.ToByte("00");
            sendobj2.Data[6] = Convert.ToByte("00");
            sendobj2.Data[7] = Convert.ToByte("00");
            if (VCI_Transmit(m_devtype, m_devind, m_canind, ref sendobj2, 1) == 0)
            {
                MessageBox.Show("发送失败", "错误",
                        MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
            System.Threading.Thread.Sleep(100);
            VCI_CAN_OBJ sendobj6 = new VCI_CAN_OBJ();
            sendobj6.ExternFlag = 1;
            sendobj6.ID = System.Convert.ToUInt32("18000029", 16);
            sendobj6.DataLen = 8;
            sendobj6.Data[0] = Convert.ToByte("191");
            sendobj6.Data[1] = Convert.ToByte("160");
            sendobj6.Data[2] = Convert.ToByte("00");
            sendobj6.Data[3] = Convert.ToByte("00");
            sendobj6.Data[4] = Convert.ToByte("00");
            sendobj6.Data[5] = Convert.ToByte("00");
            sendobj6.Data[6] = Convert.ToByte("00");
            sendobj6.Data[7] = Convert.ToByte("00");
            if (VCI_Transmit(m_devtype, m_devind, m_canind, ref sendobj6, 1) == 0)
            {
                MessageBox.Show("发送失败", "错误",
                        MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
            System.Threading.Thread.Sleep(1000);
            VCI_CAN_OBJ sendobj7 = new VCI_CAN_OBJ();
            sendobj7.ExternFlag = 1;
            sendobj7.ID = System.Convert.ToUInt32("18000029", 16);
            sendobj7.DataLen = 8;
            sendobj7.Data[0] = Convert.ToByte("191");
            sendobj7.Data[1] = Convert.ToByte("161");
            sendobj7.Data[2] = Convert.ToByte("00");
            sendobj7.Data[3] = Convert.ToByte("00");
            sendobj7.Data[4] = Convert.ToByte("00");
            sendobj7.Data[5] = Convert.ToByte("00");
            sendobj7.Data[6] = Convert.ToByte("00");
            sendobj7.Data[7] = Convert.ToByte("00");
            if (VCI_Transmit(m_devtype, m_devind, m_canind, ref sendobj7, 1) == 0)
            {
                MessageBox.Show("发送失败", "错误",
                        MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
            System.Threading.Thread.Sleep(1000);
            VCI_CAN_OBJ sendobj8 = new VCI_CAN_OBJ();
            sendobj8.ExternFlag = 1;
            sendobj8.ID = System.Convert.ToUInt32("18000029", 16);
            sendobj8.DataLen = 8;
            sendobj8.Data[0] = Convert.ToByte("191");
            sendobj8.Data[1] = Convert.ToByte("162");
            sendobj8.Data[2] = Convert.ToByte("00");
            sendobj8.Data[3] = Convert.ToByte("00");
            sendobj8.Data[4] = Convert.ToByte("00");
            sendobj8.Data[5] = Convert.ToByte("00");
            sendobj8.Data[6] = Convert.ToByte("00");
            sendobj8.Data[7] = Convert.ToByte("00");
            if (VCI_Transmit(m_devtype, m_devind, m_canind, ref sendobj8, 1) == 0)
            {
                MessageBox.Show("发送失败", "错误",
                        MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
            System.Threading.Thread.Sleep(1500);
            callback1.ID48 = temp[50]; callback1.ID49 = temp[51]; callback1.ID50 = temp[52];
            轻微漏电时间标定.Enabled = true;
        }
        unsafe private void 单体过高时间标定_Click(object sender, EventArgs e)
        {
            单体过高时间标定.Enabled = false;
            UInt32[] a = new UInt32[4];
            if (m_bOpen == 0)
                return;
            VCI_CAN_OBJ sendobj = new VCI_CAN_OBJ();
            sendobj.ExternFlag = 1;
            sendobj.ID = System.Convert.ToUInt32("18000029", 16);
            sendobj.DataLen = 8;
            sendobj.Data[0] = Convert.ToByte("127");
            sendobj.Data[1] = Convert.ToByte("163");
            sendobj.Data[2] = Convert.ToByte("00");
            sendobj.Data[3] = Convert.ToByte((Convert.ToDouble(单体过高时间1.Text)) * 10);
            sendobj.Data[4] = Convert.ToByte("00");
            sendobj.Data[5] = Convert.ToByte("00");
            sendobj.Data[6] = Convert.ToByte("00");
            sendobj.Data[7] = Convert.ToByte("00");
            if (VCI_Transmit(m_devtype, m_devind, m_canind, ref sendobj, 1) == 0)
            {
                MessageBox.Show("发送失败", "错误",
                        MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
            System.Threading.Thread.Sleep(100);

            VCI_CAN_OBJ sendobj1 = new VCI_CAN_OBJ();
            sendobj1.ExternFlag = 1;
            sendobj1.ID = System.Convert.ToUInt32("18000029", 16);
            sendobj1.DataLen = 8;
            sendobj1.Data[0] = Convert.ToByte("127");
            sendobj1.Data[1] = Convert.ToByte("164");
            sendobj1.Data[2] = Convert.ToByte("00");
            sendobj1.Data[3] = Convert.ToByte((Convert.ToDouble(单体过高时间2.Text)) * 10);
            sendobj1.Data[4] = Convert.ToByte("00");
            sendobj1.Data[5] = Convert.ToByte("00");
            sendobj1.Data[6] = Convert.ToByte("00");
            sendobj1.Data[7] = Convert.ToByte("00");
            if (VCI_Transmit(m_devtype, m_devind, m_canind, ref sendobj1, 1) == 0)
            {
                MessageBox.Show("发送失败", "错误",
                        MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
            System.Threading.Thread.Sleep(100);
            if (m_bOpen == 0)
                return;
            VCI_CAN_OBJ sendobj2 = new VCI_CAN_OBJ();
            sendobj2.ExternFlag = 1;
            sendobj2.ID = System.Convert.ToUInt32("18000029", 16);
            sendobj2.DataLen = 8;
            sendobj2.Data[0] = Convert.ToByte("127");
            sendobj2.Data[1] = Convert.ToByte("165");
            sendobj2.Data[2] = Convert.ToByte("00");
            sendobj2.Data[3] = Convert.ToByte((Convert.ToDouble(单体过高时间3.Text)) * 10);
            sendobj2.Data[4] = Convert.ToByte("00");
            sendobj2.Data[5] = Convert.ToByte("00");
            sendobj2.Data[6] = Convert.ToByte("00");
            sendobj2.Data[7] = Convert.ToByte("00");
            if (VCI_Transmit(m_devtype, m_devind, m_canind, ref sendobj2, 1) == 0)
            {
                MessageBox.Show("发送失败", "错误",
                        MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
            System.Threading.Thread.Sleep(100);
            VCI_CAN_OBJ sendobj6 = new VCI_CAN_OBJ();
            sendobj6.ExternFlag = 1;
            sendobj6.ID = System.Convert.ToUInt32("18000029", 16);
            sendobj6.DataLen = 8;
            sendobj6.Data[0] = Convert.ToByte("191");
            sendobj6.Data[1] = Convert.ToByte("163");
            sendobj6.Data[2] = Convert.ToByte("00");
            sendobj6.Data[3] = Convert.ToByte("00");
            sendobj6.Data[4] = Convert.ToByte("00");
            sendobj6.Data[5] = Convert.ToByte("00");
            sendobj6.Data[6] = Convert.ToByte("00");
            sendobj6.Data[7] = Convert.ToByte("00");
            if (VCI_Transmit(m_devtype, m_devind, m_canind, ref sendobj6, 1) == 0)
            {
                MessageBox.Show("发送失败", "错误",
                        MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
            System.Threading.Thread.Sleep(1000);
            VCI_CAN_OBJ sendobj7 = new VCI_CAN_OBJ();
            sendobj7.ExternFlag = 1;
            sendobj7.ID = System.Convert.ToUInt32("18000029", 16);
            sendobj7.DataLen = 8;
            sendobj7.Data[0] = Convert.ToByte("191");
            sendobj7.Data[1] = Convert.ToByte("164");
            sendobj7.Data[2] = Convert.ToByte("00");
            sendobj7.Data[3] = Convert.ToByte("00");
            sendobj7.Data[4] = Convert.ToByte("00");
            sendobj7.Data[5] = Convert.ToByte("00");
            sendobj7.Data[6] = Convert.ToByte("00");
            sendobj7.Data[7] = Convert.ToByte("00");
            if (VCI_Transmit(m_devtype, m_devind, m_canind, ref sendobj7, 1) == 0)
            {
                MessageBox.Show("发送失败", "错误",
                        MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
            System.Threading.Thread.Sleep(1000);
            VCI_CAN_OBJ sendobj8 = new VCI_CAN_OBJ();
            sendobj8.ExternFlag = 1;
            sendobj8.ID = System.Convert.ToUInt32("18000029", 16);
            sendobj8.DataLen = 8;
            sendobj8.Data[0] = Convert.ToByte("191");
            sendobj8.Data[1] = Convert.ToByte("165");
            sendobj8.Data[2] = Convert.ToByte("00");
            sendobj8.Data[3] = Convert.ToByte("00");
            sendobj8.Data[4] = Convert.ToByte("00");
            sendobj8.Data[5] = Convert.ToByte("00");
            sendobj8.Data[6] = Convert.ToByte("00");
            sendobj8.Data[7] = Convert.ToByte("00");
            if (VCI_Transmit(m_devtype, m_devind, m_canind, ref sendobj8, 1) == 0)
            {
                MessageBox.Show("发送失败", "错误",
                        MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
            System.Threading.Thread.Sleep(1500);
            callback1.ID51 = temp[53]; callback1.ID52 = temp[54]; callback1.ID53 = temp[55];
            单体过高时间标定.Enabled = true;

        }
       unsafe private void 单体过低时间标定_Click(object sender, EventArgs e)
        {
            单体过低时间标定.Enabled = false;
            UInt32[] a = new UInt32[4];
            if (m_bOpen == 0)
                return;
            VCI_CAN_OBJ sendobj = new VCI_CAN_OBJ();
            sendobj.ExternFlag = 1;
            sendobj.ID = System.Convert.ToUInt32("18000029", 16);
            sendobj.DataLen = 8;
            sendobj.Data[0] = Convert.ToByte("127");
            sendobj.Data[1] = Convert.ToByte("166");
            sendobj.Data[2] = Convert.ToByte("00");
            sendobj.Data[3] = Convert.ToByte((Convert.ToDouble(单体过低时间1.Text)) * 10);
            sendobj.Data[4] = Convert.ToByte("00");
            sendobj.Data[5] = Convert.ToByte("00");
            sendobj.Data[6] = Convert.ToByte("00");
            sendobj.Data[7] = Convert.ToByte("00");
            if (VCI_Transmit(m_devtype, m_devind, m_canind, ref sendobj, 1) == 0)
            {
                MessageBox.Show("发送失败", "错误",
                        MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
            System.Threading.Thread.Sleep(100);

            VCI_CAN_OBJ sendobj1 = new VCI_CAN_OBJ();
            sendobj1.ExternFlag = 1;
            sendobj1.ID = System.Convert.ToUInt32("18000029", 16);
            sendobj1.DataLen = 8;
            sendobj1.Data[0] = Convert.ToByte("127");
            sendobj1.Data[1] = Convert.ToByte("167");
            sendobj1.Data[2] = Convert.ToByte("00");
            sendobj1.Data[3] = Convert.ToByte((Convert.ToDouble(单体过低时间2.Text)) * 10);
            sendobj1.Data[4] = Convert.ToByte("00");
            sendobj1.Data[5] = Convert.ToByte("00");
            sendobj1.Data[6] = Convert.ToByte("00");
            sendobj1.Data[7] = Convert.ToByte("00");
            if (VCI_Transmit(m_devtype, m_devind, m_canind, ref sendobj1, 1) == 0)
            {
                MessageBox.Show("发送失败", "错误",
                        MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
            System.Threading.Thread.Sleep(100);
            if (m_bOpen == 0)
                return;
            VCI_CAN_OBJ sendobj2 = new VCI_CAN_OBJ();
            sendobj2.ExternFlag = 1;
            sendobj2.ID = System.Convert.ToUInt32("18000029", 16);
            sendobj2.DataLen = 8;
            sendobj2.Data[0] = Convert.ToByte("127");
            sendobj2.Data[1] = Convert.ToByte("168");
            sendobj2.Data[2] = Convert.ToByte("00");
            sendobj2.Data[3] = Convert.ToByte((Convert.ToDouble(单体过低时间3.Text)) * 10);
            sendobj2.Data[4] = Convert.ToByte("00");
            sendobj2.Data[5] = Convert.ToByte("00");
            sendobj2.Data[6] = Convert.ToByte("00");
            sendobj2.Data[7] = Convert.ToByte("00");
            if (VCI_Transmit(m_devtype, m_devind, m_canind, ref sendobj2, 1) == 0)
            {
                MessageBox.Show("发送失败", "错误",
                        MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
            System.Threading.Thread.Sleep(100);
            VCI_CAN_OBJ sendobj6 = new VCI_CAN_OBJ();
            sendobj6.ExternFlag = 1;
            sendobj6.ID = System.Convert.ToUInt32("18000029", 16);
            sendobj6.DataLen = 8;
            sendobj6.Data[0] = Convert.ToByte("191");
            sendobj6.Data[1] = Convert.ToByte("166");
            sendobj6.Data[2] = Convert.ToByte("00");
            sendobj6.Data[3] = Convert.ToByte("00");
            sendobj6.Data[4] = Convert.ToByte("00");
            sendobj6.Data[5] = Convert.ToByte("00");
            sendobj6.Data[6] = Convert.ToByte("00");
            sendobj6.Data[7] = Convert.ToByte("00");
            if (VCI_Transmit(m_devtype, m_devind, m_canind, ref sendobj6, 1) == 0)
            {
                MessageBox.Show("发送失败", "错误",
                        MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
            System.Threading.Thread.Sleep(1000);
            VCI_CAN_OBJ sendobj7 = new VCI_CAN_OBJ();
            sendobj7.ExternFlag = 1;
            sendobj7.ID = System.Convert.ToUInt32("18000029", 16);
            sendobj7.DataLen = 8;
            sendobj7.Data[0] = Convert.ToByte("191");
            sendobj7.Data[1] = Convert.ToByte("167");
            sendobj7.Data[2] = Convert.ToByte("00");
            sendobj7.Data[3] = Convert.ToByte("00");
            sendobj7.Data[4] = Convert.ToByte("00");
            sendobj7.Data[5] = Convert.ToByte("00");
            sendobj7.Data[6] = Convert.ToByte("00");
            sendobj7.Data[7] = Convert.ToByte("00");
            if (VCI_Transmit(m_devtype, m_devind, m_canind, ref sendobj7, 1) == 0)
            {
                MessageBox.Show("发送失败", "错误",
                        MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
            System.Threading.Thread.Sleep(1000);
            VCI_CAN_OBJ sendobj8 = new VCI_CAN_OBJ();
            sendobj8.ExternFlag = 1;
            sendobj8.ID = System.Convert.ToUInt32("18000029", 16);
            sendobj8.DataLen = 8;
            sendobj8.Data[0] = Convert.ToByte("191");
            sendobj8.Data[1] = Convert.ToByte("168");
            sendobj8.Data[2] = Convert.ToByte("00");
            sendobj8.Data[3] = Convert.ToByte("00");
            sendobj8.Data[4] = Convert.ToByte("00");
            sendobj8.Data[5] = Convert.ToByte("00");
            sendobj8.Data[6] = Convert.ToByte("00");
            sendobj8.Data[7] = Convert.ToByte("00");
            if (VCI_Transmit(m_devtype, m_devind, m_canind, ref sendobj8, 1) == 0)
            {
                MessageBox.Show("发送失败", "错误",
                        MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
            System.Threading.Thread.Sleep(1500);
            callback1.ID54 = temp[56]; callback1.ID55 = temp[57]; callback1.ID56 = temp[58];
            单体过低时间标定.Enabled = true;
        }
        unsafe private void 总压过高时间标定_Click(object sender, EventArgs e)
        {
            总压过高时间标定.Enabled = false;
            UInt32[] a = new UInt32[4];
            if (m_bOpen == 0)
                return;
            VCI_CAN_OBJ sendobj = new VCI_CAN_OBJ();
            sendobj.ExternFlag = 1;
            sendobj.ID = System.Convert.ToUInt32("18000029", 16);
            sendobj.DataLen = 8;
            sendobj.Data[0] = Convert.ToByte("127");
            sendobj.Data[1] = Convert.ToByte("169");
            sendobj.Data[2] = Convert.ToByte("00");
            sendobj.Data[3] = Convert.ToByte((Convert.ToDouble(总压过高时间1.Text)) * 10);
            sendobj.Data[4] = Convert.ToByte("00");
            sendobj.Data[5] = Convert.ToByte("00");
            sendobj.Data[6] = Convert.ToByte("00");
            sendobj.Data[7] = Convert.ToByte("00");
            if (VCI_Transmit(m_devtype, m_devind, m_canind, ref sendobj, 1) == 0)
            {
                MessageBox.Show("发送失败", "错误",
                        MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
            System.Threading.Thread.Sleep(100);

            VCI_CAN_OBJ sendobj1 = new VCI_CAN_OBJ();
            sendobj1.ExternFlag = 1;
            sendobj1.ID = System.Convert.ToUInt32("18000029", 16);
            sendobj1.DataLen = 8;
            sendobj1.Data[0] = Convert.ToByte("127");
            sendobj1.Data[1] = Convert.ToByte("170");
            sendobj1.Data[2] = Convert.ToByte("00");
            sendobj1.Data[3] = Convert.ToByte((Convert.ToDouble(总压过高时间2.Text)) * 10);
            sendobj1.Data[4] = Convert.ToByte("00");
            sendobj1.Data[5] = Convert.ToByte("00");
            sendobj1.Data[6] = Convert.ToByte("00");
            sendobj1.Data[7] = Convert.ToByte("00");
            if (VCI_Transmit(m_devtype, m_devind, m_canind, ref sendobj1, 1) == 0)
            {
                MessageBox.Show("发送失败", "错误",
                        MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
            System.Threading.Thread.Sleep(100);
            if (m_bOpen == 0)
                return;
            VCI_CAN_OBJ sendobj2 = new VCI_CAN_OBJ();
            sendobj2.ExternFlag = 1;
            sendobj2.ID = System.Convert.ToUInt32("18000029", 16);
            sendobj2.DataLen = 8;
            sendobj2.Data[0] = Convert.ToByte("127");
            sendobj2.Data[1] = Convert.ToByte("171");
            sendobj2.Data[2] = Convert.ToByte("00");
            sendobj2.Data[3] = Convert.ToByte((Convert.ToDouble(总压过高时间3.Text)) * 10);
            sendobj2.Data[4] = Convert.ToByte("00");
            sendobj2.Data[5] = Convert.ToByte("00");
            sendobj2.Data[6] = Convert.ToByte("00");
            sendobj2.Data[7] = Convert.ToByte("00");
            if (VCI_Transmit(m_devtype, m_devind, m_canind, ref sendobj2, 1) == 0)
            {
                MessageBox.Show("发送失败", "错误",
                        MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
            System.Threading.Thread.Sleep(100);
            VCI_CAN_OBJ sendobj6 = new VCI_CAN_OBJ();
            sendobj6.ExternFlag = 1;
            sendobj6.ID = System.Convert.ToUInt32("18000029", 16);
            sendobj6.DataLen = 8;
            sendobj6.Data[0] = Convert.ToByte("191");
            sendobj6.Data[1] = Convert.ToByte("169");
            sendobj6.Data[2] = Convert.ToByte("00");
            sendobj6.Data[3] = Convert.ToByte("00");
            sendobj6.Data[4] = Convert.ToByte("00");
            sendobj6.Data[5] = Convert.ToByte("00");
            sendobj6.Data[6] = Convert.ToByte("00");
            sendobj6.Data[7] = Convert.ToByte("00");
            if (VCI_Transmit(m_devtype, m_devind, m_canind, ref sendobj6, 1) == 0)
            {
                MessageBox.Show("发送失败", "错误",
                        MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
            System.Threading.Thread.Sleep(1500);
            VCI_CAN_OBJ sendobj7 = new VCI_CAN_OBJ();
            sendobj7.ExternFlag = 1;
            sendobj7.ID = System.Convert.ToUInt32("18000029", 16);
            sendobj7.DataLen = 8;
            sendobj7.Data[0] = Convert.ToByte("191");
            sendobj7.Data[1] = Convert.ToByte("170");
            sendobj7.Data[2] = Convert.ToByte("00");
            sendobj7.Data[3] = Convert.ToByte("00");
            sendobj7.Data[4] = Convert.ToByte("00");
            sendobj7.Data[5] = Convert.ToByte("00");
            sendobj7.Data[6] = Convert.ToByte("00");
            sendobj7.Data[7] = Convert.ToByte("00");
            if (VCI_Transmit(m_devtype, m_devind, m_canind, ref sendobj7, 1) == 0)
            {
                MessageBox.Show("发送失败", "错误",
                        MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
            System.Threading.Thread.Sleep(1500);
            VCI_CAN_OBJ sendobj8 = new VCI_CAN_OBJ();
            sendobj8.ExternFlag = 1;
            sendobj8.ID = System.Convert.ToUInt32("18000029", 16);
            sendobj8.DataLen = 8;
            sendobj8.Data[0] = Convert.ToByte("191");
            sendobj8.Data[1] = Convert.ToByte("171");
            sendobj8.Data[2] = Convert.ToByte("00");
            sendobj8.Data[3] = Convert.ToByte("00");
            sendobj8.Data[4] = Convert.ToByte("00");
            sendobj8.Data[5] = Convert.ToByte("00");
            sendobj8.Data[6] = Convert.ToByte("00");
            sendobj8.Data[7] = Convert.ToByte("00");
            if (VCI_Transmit(m_devtype, m_devind, m_canind, ref sendobj8, 1) == 0)
            {
                MessageBox.Show("发送失败", "错误",
                        MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
            System.Threading.Thread.Sleep(1500);
            callback1.ID57 = temp[59]; callback1.ID58 = temp[60]; callback1.ID59 = temp[61];
            总压过高时间标定.Enabled = true;

        }
       unsafe private void 总压过低时间标定_Click(object sender, EventArgs e)
        {
            总压过低时间标定.Enabled = false;
            UInt32[] a = new UInt32[4];
            if (m_bOpen == 0)
                return;
            VCI_CAN_OBJ sendobj = new VCI_CAN_OBJ();
            sendobj.ExternFlag = 1;
            sendobj.ID = System.Convert.ToUInt32("18000029", 16);
            sendobj.DataLen = 8;
            sendobj.Data[0] = Convert.ToByte("127");
            sendobj.Data[1] = Convert.ToByte("172");
            sendobj.Data[2] = Convert.ToByte("00");
            sendobj.Data[3] = Convert.ToByte((Convert.ToDouble(总压过低时间1.Text)) * 10);
            sendobj.Data[4] = Convert.ToByte("00");
            sendobj.Data[5] = Convert.ToByte("00");
            sendobj.Data[6] = Convert.ToByte("00");
            sendobj.Data[7] = Convert.ToByte("00");
            if (VCI_Transmit(m_devtype, m_devind, m_canind, ref sendobj, 1) == 0)
            {
                MessageBox.Show("发送失败", "错误",
                        MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
            System.Threading.Thread.Sleep(100);

            VCI_CAN_OBJ sendobj1 = new VCI_CAN_OBJ();
            sendobj1.ExternFlag = 1;
            sendobj1.ID = System.Convert.ToUInt32("18000029", 16);
            sendobj1.DataLen = 8;
            sendobj1.Data[0] = Convert.ToByte("127");
            sendobj1.Data[1] = Convert.ToByte("173");
            sendobj1.Data[2] = Convert.ToByte("00");
            sendobj1.Data[3] = Convert.ToByte((Convert.ToDouble(总压过低时间2.Text)) * 10);
            sendobj1.Data[4] = Convert.ToByte("00");
            sendobj1.Data[5] = Convert.ToByte("00");
            sendobj1.Data[6] = Convert.ToByte("00");
            sendobj1.Data[7] = Convert.ToByte("00");
            if (VCI_Transmit(m_devtype, m_devind, m_canind, ref sendobj1, 1) == 0)
            {
                MessageBox.Show("发送失败", "错误",
                        MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
            System.Threading.Thread.Sleep(100);
            if (m_bOpen == 0)
                return;
            VCI_CAN_OBJ sendobj2 = new VCI_CAN_OBJ();
            sendobj2.ExternFlag = 1;
            sendobj2.ID = System.Convert.ToUInt32("18000029", 16);
            sendobj2.DataLen = 8;
            sendobj2.Data[0] = Convert.ToByte("127");
            sendobj2.Data[1] = Convert.ToByte("174");
            sendobj2.Data[2] = Convert.ToByte("00");
            sendobj2.Data[3] = Convert.ToByte((Convert.ToDouble(总压过低时间3.Text)) * 10);
            sendobj2.Data[4] = Convert.ToByte("00");
            sendobj2.Data[5] = Convert.ToByte("00");
            sendobj2.Data[6] = Convert.ToByte("00");
            sendobj2.Data[7] = Convert.ToByte("00");
            if (VCI_Transmit(m_devtype, m_devind, m_canind, ref sendobj2, 1) == 0)
            {
                MessageBox.Show("发送失败", "错误",
                        MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
            System.Threading.Thread.Sleep(100);
            VCI_CAN_OBJ sendobj6 = new VCI_CAN_OBJ();
            sendobj6.ExternFlag = 1;
            sendobj6.ID = System.Convert.ToUInt32("18000029", 16);
            sendobj6.DataLen = 8;
            sendobj6.Data[0] = Convert.ToByte("191");
            sendobj6.Data[1] = Convert.ToByte("172");
            sendobj6.Data[2] = Convert.ToByte("00");
            sendobj6.Data[3] = Convert.ToByte("00");
            sendobj6.Data[4] = Convert.ToByte("00");
            sendobj6.Data[5] = Convert.ToByte("00");
            sendobj6.Data[6] = Convert.ToByte("00");
            sendobj6.Data[7] = Convert.ToByte("00");
            if (VCI_Transmit(m_devtype, m_devind, m_canind, ref sendobj6, 1) == 0)
            {
                MessageBox.Show("发送失败", "错误",
                        MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
            System.Threading.Thread.Sleep(1500);
            VCI_CAN_OBJ sendobj7 = new VCI_CAN_OBJ();
            sendobj7.ExternFlag = 1;
            sendobj7.ID = System.Convert.ToUInt32("18000029", 16);
            sendobj7.DataLen = 8;
            sendobj7.Data[0] = Convert.ToByte("191");
            sendobj7.Data[1] = Convert.ToByte("173");
            sendobj7.Data[2] = Convert.ToByte("00");
            sendobj7.Data[3] = Convert.ToByte("00");
            sendobj7.Data[4] = Convert.ToByte("00");
            sendobj7.Data[5] = Convert.ToByte("00");
            sendobj7.Data[6] = Convert.ToByte("00");
            sendobj7.Data[7] = Convert.ToByte("00");
            if (VCI_Transmit(m_devtype, m_devind, m_canind, ref sendobj7, 1) == 0)
            {
                MessageBox.Show("发送失败", "错误",
                        MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
            System.Threading.Thread.Sleep(1500);
            VCI_CAN_OBJ sendobj8 = new VCI_CAN_OBJ();
            sendobj8.ExternFlag = 1;
            sendobj8.ID = System.Convert.ToUInt32("18000029", 16);
            sendobj8.DataLen = 8;
            sendobj8.Data[0] = Convert.ToByte("191");
            sendobj8.Data[1] = Convert.ToByte("174");
            sendobj8.Data[2] = Convert.ToByte("00");
            sendobj8.Data[3] = Convert.ToByte("00");
            sendobj8.Data[4] = Convert.ToByte("00");
            sendobj8.Data[5] = Convert.ToByte("00");
            sendobj8.Data[6] = Convert.ToByte("00");
            sendobj8.Data[7] = Convert.ToByte("00");
            if (VCI_Transmit(m_devtype, m_devind, m_canind, ref sendobj8, 1) == 0)
            {
                MessageBox.Show("发送失败", "错误",
                        MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
            System.Threading.Thread.Sleep(1500);
            callback1.ID60 = temp[62]; callback1.ID61 = temp[63]; callback1.ID62 = temp[64];
            总压过低时间标定.Enabled = true;
        }
       unsafe private void MOS温度过高时间标定_Click(object sender, EventArgs e)
        {
            MOS温度过高时间标定.Enabled = false;
            UInt32[] a = new UInt32[4];
            if (m_bOpen == 0)
                return;
            VCI_CAN_OBJ sendobj = new VCI_CAN_OBJ();
            sendobj.ExternFlag = 1;
            sendobj.ID = System.Convert.ToUInt32("18000029", 16);
            sendobj.DataLen = 8;
            sendobj.Data[0] = Convert.ToByte("127");
            sendobj.Data[1] = Convert.ToByte("175");
            sendobj.Data[2] = Convert.ToByte("00");
            sendobj.Data[3] = Convert.ToByte((Convert.ToDouble(MOS温度过高时间1.Text)) * 10);
            sendobj.Data[4] = Convert.ToByte("00");
            sendobj.Data[5] = Convert.ToByte("00");
            sendobj.Data[6] = Convert.ToByte("00");
            sendobj.Data[7] = Convert.ToByte("00");
            if (VCI_Transmit(m_devtype, m_devind, m_canind, ref sendobj, 1) == 0)
            {
                MessageBox.Show("发送失败", "错误",
                        MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
            System.Threading.Thread.Sleep(100);

            VCI_CAN_OBJ sendobj1 = new VCI_CAN_OBJ();
            sendobj1.ExternFlag = 1;
            sendobj1.ID = System.Convert.ToUInt32("18000029", 16);
            sendobj1.DataLen = 8;
            sendobj1.Data[0] = Convert.ToByte("127");
            sendobj1.Data[1] = Convert.ToByte("176");
            sendobj1.Data[2] = Convert.ToByte("00");
            sendobj1.Data[3] = Convert.ToByte((Convert.ToDouble(MOS温度过高时间2.Text)) * 10);
            sendobj1.Data[4] = Convert.ToByte("00");
            sendobj1.Data[5] = Convert.ToByte("00");
            sendobj1.Data[6] = Convert.ToByte("00");
            sendobj1.Data[7] = Convert.ToByte("00");
            if (VCI_Transmit(m_devtype, m_devind, m_canind, ref sendobj1, 1) == 0)
            {
                MessageBox.Show("发送失败", "错误",
                        MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
            System.Threading.Thread.Sleep(100);
            if (m_bOpen == 0)
                return;
            VCI_CAN_OBJ sendobj2 = new VCI_CAN_OBJ();
            sendobj2.ExternFlag = 1;
            sendobj2.ID = System.Convert.ToUInt32("18000029", 16);
            sendobj2.DataLen = 8;
            sendobj2.Data[0] = Convert.ToByte("127");
            sendobj2.Data[1] = Convert.ToByte("177");
            sendobj2.Data[2] = Convert.ToByte("00");
            sendobj2.Data[3] = Convert.ToByte((Convert.ToDouble(MOS温度过高时间3.Text)) * 10);
            sendobj2.Data[4] = Convert.ToByte("00");
            sendobj2.Data[5] = Convert.ToByte("00");
            sendobj2.Data[6] = Convert.ToByte("00");
            sendobj2.Data[7] = Convert.ToByte("00");
            if (VCI_Transmit(m_devtype, m_devind, m_canind, ref sendobj2, 1) == 0)
            {
                MessageBox.Show("发送失败", "错误",
                        MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
            System.Threading.Thread.Sleep(100);
            VCI_CAN_OBJ sendobj6 = new VCI_CAN_OBJ();
            sendobj6.ExternFlag = 1;
            sendobj6.ID = System.Convert.ToUInt32("18000029", 16);
            sendobj6.DataLen = 8;
            sendobj6.Data[0] = Convert.ToByte("191");
            sendobj6.Data[1] = Convert.ToByte("175");
            sendobj6.Data[2] = Convert.ToByte("00");
            sendobj6.Data[3] = Convert.ToByte("00");
            sendobj6.Data[4] = Convert.ToByte("00");
            sendobj6.Data[5] = Convert.ToByte("00");
            sendobj6.Data[6] = Convert.ToByte("00");
            sendobj6.Data[7] = Convert.ToByte("00");
            if (VCI_Transmit(m_devtype, m_devind, m_canind, ref sendobj6, 1) == 0)
            {
                MessageBox.Show("发送失败", "错误",
                        MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
            System.Threading.Thread.Sleep(1500);
            VCI_CAN_OBJ sendobj7 = new VCI_CAN_OBJ();
            sendobj7.ExternFlag = 1;
            sendobj7.ID = System.Convert.ToUInt32("18000029", 16);
            sendobj7.DataLen = 8;
            sendobj7.Data[0] = Convert.ToByte("191");
            sendobj7.Data[1] = Convert.ToByte("176");
            sendobj7.Data[2] = Convert.ToByte("00");
            sendobj7.Data[3] = Convert.ToByte("00");
            sendobj7.Data[4] = Convert.ToByte("00");
            sendobj7.Data[5] = Convert.ToByte("00");
            sendobj7.Data[6] = Convert.ToByte("00");
            sendobj7.Data[7] = Convert.ToByte("00");
            if (VCI_Transmit(m_devtype, m_devind, m_canind, ref sendobj7, 1) == 0)
            {
                MessageBox.Show("发送失败", "错误",
                        MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
            System.Threading.Thread.Sleep(1500);
            VCI_CAN_OBJ sendobj8 = new VCI_CAN_OBJ();
            sendobj8.ExternFlag = 1;
            sendobj8.ID = System.Convert.ToUInt32("18000029", 16);
            sendobj8.DataLen = 8;
            sendobj8.Data[0] = Convert.ToByte("191");
            sendobj8.Data[1] = Convert.ToByte("177");
            sendobj8.Data[2] = Convert.ToByte("00");
            sendobj8.Data[3] = Convert.ToByte("00");
            sendobj8.Data[4] = Convert.ToByte("00");
            sendobj8.Data[5] = Convert.ToByte("00");
            sendobj8.Data[6] = Convert.ToByte("00");
            sendobj8.Data[7] = Convert.ToByte("00");
            if (VCI_Transmit(m_devtype, m_devind, m_canind, ref sendobj8, 1) == 0)
            {
                MessageBox.Show("发送失败", "错误",
                        MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
            System.Threading.Thread.Sleep(1500);
            callback1.ID63 = temp[65]; callback1.ID64 = temp[66]; callback1.ID65 = temp[67];
            MOS温度过高时间标定.Enabled = true;
        }
        unsafe private void 充电电流过高时间标定_Click(object sender, EventArgs e)
        {
            充电电流过高时间标定.Enabled = false;
            UInt32[] a = new UInt32[4];
            if (m_bOpen == 0)
                return;
            VCI_CAN_OBJ sendobj = new VCI_CAN_OBJ();
            sendobj.ExternFlag = 1;
            sendobj.ID = System.Convert.ToUInt32("18000029", 16);
            sendobj.DataLen = 8;
            sendobj.Data[0] = Convert.ToByte("127");
            sendobj.Data[1] = Convert.ToByte("178");
            sendobj.Data[2] = Convert.ToByte("00");
            sendobj.Data[3] = Convert.ToByte((Convert.ToDouble(充电电流过高时间1.Text)) * 10);
            sendobj.Data[4] = Convert.ToByte("00");
            sendobj.Data[5] = Convert.ToByte("00");
            sendobj.Data[6] = Convert.ToByte("00");
            sendobj.Data[7] = Convert.ToByte("00");
            if (VCI_Transmit(m_devtype, m_devind, m_canind, ref sendobj, 1) == 0)
            {
                MessageBox.Show("发送失败", "错误",
                        MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
            System.Threading.Thread.Sleep(100);

            VCI_CAN_OBJ sendobj1 = new VCI_CAN_OBJ();
            sendobj1.ExternFlag = 1;
            sendobj1.ID = System.Convert.ToUInt32("18000029", 16);
            sendobj1.DataLen = 8;
            sendobj1.Data[0] = Convert.ToByte("127");
            sendobj1.Data[1] = Convert.ToByte("179");
            sendobj1.Data[2] = Convert.ToByte("00");
            sendobj1.Data[3] = Convert.ToByte((Convert.ToDouble(充电电流过高时间2.Text)) * 10);
            sendobj1.Data[4] = Convert.ToByte("00");
            sendobj1.Data[5] = Convert.ToByte("00");
            sendobj1.Data[6] = Convert.ToByte("00");
            sendobj1.Data[7] = Convert.ToByte("00");
            if (VCI_Transmit(m_devtype, m_devind, m_canind, ref sendobj1, 1) == 0)
            {
                MessageBox.Show("发送失败", "错误",
                        MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
            System.Threading.Thread.Sleep(100);
            if (m_bOpen == 0)
                return;
            VCI_CAN_OBJ sendobj2 = new VCI_CAN_OBJ();
            sendobj2.ExternFlag = 1;
            sendobj2.ID = System.Convert.ToUInt32("18000029", 16);
            sendobj2.DataLen = 8;
            sendobj2.Data[0] = Convert.ToByte("127");
            sendobj2.Data[1] = Convert.ToByte("180");
            sendobj2.Data[2] = Convert.ToByte("00");
            sendobj2.Data[3] = Convert.ToByte((Convert.ToDouble(充电电流过高时间3.Text)) * 10);
            sendobj2.Data[4] = Convert.ToByte("00");
            sendobj2.Data[5] = Convert.ToByte("00");
            sendobj2.Data[6] = Convert.ToByte("00");
            sendobj2.Data[7] = Convert.ToByte("00");
            if (VCI_Transmit(m_devtype, m_devind, m_canind, ref sendobj2, 1) == 0)
            {
                MessageBox.Show("发送失败", "错误",
                        MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
            System.Threading.Thread.Sleep(100);
            VCI_CAN_OBJ sendobj6 = new VCI_CAN_OBJ();
            sendobj6.ExternFlag = 1;
            sendobj6.ID = System.Convert.ToUInt32("18000029", 16);
            sendobj6.DataLen = 8;
            sendobj6.Data[0] = Convert.ToByte("191");
            sendobj6.Data[1] = Convert.ToByte("178");
            sendobj6.Data[2] = Convert.ToByte("00");
            sendobj6.Data[3] = Convert.ToByte("00");
            sendobj6.Data[4] = Convert.ToByte("00");
            sendobj6.Data[5] = Convert.ToByte("00");
            sendobj6.Data[6] = Convert.ToByte("00");
            sendobj6.Data[7] = Convert.ToByte("00");
            if (VCI_Transmit(m_devtype, m_devind, m_canind, ref sendobj6, 1) == 0)
            {
                MessageBox.Show("发送失败", "错误",
                        MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
            System.Threading.Thread.Sleep(1500);
            VCI_CAN_OBJ sendobj7 = new VCI_CAN_OBJ();
            sendobj7.ExternFlag = 1;
            sendobj7.ID = System.Convert.ToUInt32("18000029", 16);
            sendobj7.DataLen = 8;
            sendobj7.Data[0] = Convert.ToByte("191");
            sendobj7.Data[1] = Convert.ToByte("179");
            sendobj7.Data[2] = Convert.ToByte("00");
            sendobj7.Data[3] = Convert.ToByte("00");
            sendobj7.Data[4] = Convert.ToByte("00");
            sendobj7.Data[5] = Convert.ToByte("00");
            sendobj7.Data[6] = Convert.ToByte("00");
            sendobj7.Data[7] = Convert.ToByte("00");
            if (VCI_Transmit(m_devtype, m_devind, m_canind, ref sendobj7, 1) == 0)
            {
                MessageBox.Show("发送失败", "错误",
                        MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
            System.Threading.Thread.Sleep(1500);
            VCI_CAN_OBJ sendobj8 = new VCI_CAN_OBJ();
            sendobj8.ExternFlag = 1;
            sendobj8.ID = System.Convert.ToUInt32("18000029", 16);
            sendobj8.DataLen = 8;
            sendobj8.Data[0] = Convert.ToByte("191");
            sendobj8.Data[1] = Convert.ToByte("180");
            sendobj8.Data[2] = Convert.ToByte("00");
            sendobj8.Data[3] = Convert.ToByte("00");
            sendobj8.Data[4] = Convert.ToByte("00");
            sendobj8.Data[5] = Convert.ToByte("00");
            sendobj8.Data[6] = Convert.ToByte("00");
            sendobj8.Data[7] = Convert.ToByte("00");
            if (VCI_Transmit(m_devtype, m_devind, m_canind, ref sendobj8, 1) == 0)
            {
                MessageBox.Show("发送失败", "错误",
                        MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
            System.Threading.Thread.Sleep(1500);
            callback1.ID66 = temp[68]; callback1.ID67 = temp[69]; callback1.ID68 = temp[70];
            充电电流过高时间标定.Enabled = true;

        }
        unsafe private void 放电电流过高时间标定_Click(object sender, EventArgs e)
        {
            放电电流过高时间标定.Enabled = false;
            UInt32[] a = new UInt32[4];
            if (m_bOpen == 0)
                return;
            VCI_CAN_OBJ sendobj = new VCI_CAN_OBJ();
            sendobj.ExternFlag = 1;
            sendobj.ID = System.Convert.ToUInt32("18000029", 16);
            sendobj.DataLen = 8;
            sendobj.Data[0] = Convert.ToByte("127");
            sendobj.Data[1] = Convert.ToByte("181");
            sendobj.Data[2] = Convert.ToByte("00");
            sendobj.Data[3] = Convert.ToByte((Convert.ToDouble(放电电流过高时间1.Text)) * 10);
            sendobj.Data[4] = Convert.ToByte("00");
            sendobj.Data[5] = Convert.ToByte("00");
            sendobj.Data[6] = Convert.ToByte("00");
            sendobj.Data[7] = Convert.ToByte("00");
            if (VCI_Transmit(m_devtype, m_devind, m_canind, ref sendobj, 1) == 0)
            {
                MessageBox.Show("发送失败", "错误",
                        MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
            System.Threading.Thread.Sleep(100);

            VCI_CAN_OBJ sendobj1 = new VCI_CAN_OBJ();
            sendobj1.ExternFlag = 1;
            sendobj1.ID = System.Convert.ToUInt32("18000029", 16);
            sendobj1.DataLen = 8;
            sendobj1.Data[0] = Convert.ToByte("127");
            sendobj1.Data[1] = Convert.ToByte("182");
            sendobj1.Data[2] = Convert.ToByte("00");
            sendobj1.Data[3] = Convert.ToByte((Convert.ToDouble(放电电流过高时间2.Text)) * 10);
            sendobj1.Data[4] = Convert.ToByte("00");
            sendobj1.Data[5] = Convert.ToByte("00");
            sendobj1.Data[6] = Convert.ToByte("00");
            sendobj1.Data[7] = Convert.ToByte("00");
            if (VCI_Transmit(m_devtype, m_devind, m_canind, ref sendobj1, 1) == 0)
            {
                MessageBox.Show("发送失败", "错误",
                        MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
            System.Threading.Thread.Sleep(100);
            if (m_bOpen == 0)
                return;
            VCI_CAN_OBJ sendobj2 = new VCI_CAN_OBJ();
            sendobj2.ExternFlag = 1;
            sendobj2.ID = System.Convert.ToUInt32("18000029", 16);
            sendobj2.DataLen = 8;
            sendobj2.Data[0] = Convert.ToByte("127");
            sendobj2.Data[1] = Convert.ToByte("183");
            sendobj2.Data[2] = Convert.ToByte("00");
            sendobj2.Data[3] = Convert.ToByte((Convert.ToDouble(放电电流过高时间3.Text)) * 10);
            sendobj2.Data[4] = Convert.ToByte("00");
            sendobj2.Data[5] = Convert.ToByte("00");
            sendobj2.Data[6] = Convert.ToByte("00");
            sendobj2.Data[7] = Convert.ToByte("00");
            if (VCI_Transmit(m_devtype, m_devind, m_canind, ref sendobj2, 1) == 0)
            {
                MessageBox.Show("发送失败", "错误",
                        MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
            System.Threading.Thread.Sleep(100);
            VCI_CAN_OBJ sendobj6 = new VCI_CAN_OBJ();
            sendobj6.ExternFlag = 1;
            sendobj6.ID = System.Convert.ToUInt32("18000029", 16);
            sendobj6.DataLen = 8;
            sendobj6.Data[0] = Convert.ToByte("191");
            sendobj6.Data[1] = Convert.ToByte("181");
            sendobj6.Data[2] = Convert.ToByte("00");
            sendobj6.Data[3] = Convert.ToByte("00");
            sendobj6.Data[4] = Convert.ToByte("00");
            sendobj6.Data[5] = Convert.ToByte("00");
            sendobj6.Data[6] = Convert.ToByte("00");
            sendobj6.Data[7] = Convert.ToByte("00");
            if (VCI_Transmit(m_devtype, m_devind, m_canind, ref sendobj6, 1) == 0)
            {
                MessageBox.Show("发送失败", "错误",
                        MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
            System.Threading.Thread.Sleep(1500);
            VCI_CAN_OBJ sendobj7 = new VCI_CAN_OBJ();
            sendobj7.ExternFlag = 1;
            sendobj7.ID = System.Convert.ToUInt32("18000029", 16);
            sendobj7.DataLen = 8;
            sendobj7.Data[0] = Convert.ToByte("191");
            sendobj7.Data[1] = Convert.ToByte("182");
            sendobj7.Data[2] = Convert.ToByte("00");
            sendobj7.Data[3] = Convert.ToByte("00");
            sendobj7.Data[4] = Convert.ToByte("00");
            sendobj7.Data[5] = Convert.ToByte("00");
            sendobj7.Data[6] = Convert.ToByte("00");
            sendobj7.Data[7] = Convert.ToByte("00");
            if (VCI_Transmit(m_devtype, m_devind, m_canind, ref sendobj7, 1) == 0)
            {
                MessageBox.Show("发送失败", "错误",
                        MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
            System.Threading.Thread.Sleep(1500);
            VCI_CAN_OBJ sendobj8 = new VCI_CAN_OBJ();
            sendobj8.ExternFlag = 1;
            sendobj8.ID = System.Convert.ToUInt32("18000029", 16);
            sendobj8.DataLen = 8;
            sendobj8.Data[0] = Convert.ToByte("191");
            sendobj8.Data[1] = Convert.ToByte("183");
            sendobj8.Data[2] = Convert.ToByte("00");
            sendobj8.Data[3] = Convert.ToByte("00");
            sendobj8.Data[4] = Convert.ToByte("00");
            sendobj8.Data[5] = Convert.ToByte("00");
            sendobj8.Data[6] = Convert.ToByte("00");
            sendobj8.Data[7] = Convert.ToByte("00");
            if (VCI_Transmit(m_devtype, m_devind, m_canind, ref sendobj8, 1) == 0)
            {
                MessageBox.Show("发送失败", "错误",
                        MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
            System.Threading.Thread.Sleep(1500);
            callback1.ID69 = temp[71]; callback1.ID70 = temp[72]; callback1.ID71 = temp[73];
            放电电流过高时间标定.Enabled = true;
        }
        unsafe private void 充电温度过高时间标定_Click(object sender, EventArgs e)
        {
            充电温度过高时间标定.Enabled = false;
            UInt32[] a = new UInt32[4];
            if (m_bOpen == 0)
                return;
            VCI_CAN_OBJ sendobj = new VCI_CAN_OBJ();
            sendobj.ExternFlag = 1;
            sendobj.ID = System.Convert.ToUInt32("18000029", 16);
            sendobj.DataLen = 8;
            sendobj.Data[0] = Convert.ToByte("127");
            sendobj.Data[1] = Convert.ToByte("184");
            sendobj.Data[2] = Convert.ToByte("00");
            sendobj.Data[3] = Convert.ToByte((Convert.ToDouble(充电温度过高时间1.Text)) * 10);
            sendobj.Data[4] = Convert.ToByte("00");
            sendobj.Data[5] = Convert.ToByte("00");
            sendobj.Data[6] = Convert.ToByte("00");
            sendobj.Data[7] = Convert.ToByte("00");
            if (VCI_Transmit(m_devtype, m_devind, m_canind, ref sendobj, 1) == 0)
            {
                MessageBox.Show("发送失败", "错误",
                        MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
            System.Threading.Thread.Sleep(100);

            VCI_CAN_OBJ sendobj1 = new VCI_CAN_OBJ();
            sendobj1.ExternFlag = 1;
            sendobj1.ID = System.Convert.ToUInt32("18000029", 16);
            sendobj1.DataLen = 8;
            sendobj1.Data[0] = Convert.ToByte("127");
            sendobj1.Data[1] = Convert.ToByte("185");
            sendobj1.Data[2] = Convert.ToByte("00");
            sendobj1.Data[3] = Convert.ToByte((Convert.ToDouble(充电温度过高时间2.Text)) * 10);
            sendobj1.Data[4] = Convert.ToByte("00");
            sendobj1.Data[5] = Convert.ToByte("00");
            sendobj1.Data[6] = Convert.ToByte("00");
            sendobj1.Data[7] = Convert.ToByte("00");
            if (VCI_Transmit(m_devtype, m_devind, m_canind, ref sendobj1, 1) == 0)
            {
                MessageBox.Show("发送失败", "错误",
                        MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
            System.Threading.Thread.Sleep(100);
            if (m_bOpen == 0)
                return;
            VCI_CAN_OBJ sendobj2 = new VCI_CAN_OBJ();
            sendobj2.ExternFlag = 1;
            sendobj2.ID = System.Convert.ToUInt32("18000029", 16);
            sendobj2.DataLen = 8;
            sendobj2.Data[0] = Convert.ToByte("127");
            sendobj2.Data[1] = Convert.ToByte("186");
            sendobj2.Data[2] = Convert.ToByte("00");
            sendobj2.Data[3] = Convert.ToByte((Convert.ToDouble(充电温度过高时间3.Text)) * 10);
            sendobj2.Data[4] = Convert.ToByte("00");
            sendobj2.Data[5] = Convert.ToByte("00");
            sendobj2.Data[6] = Convert.ToByte("00");
            sendobj2.Data[7] = Convert.ToByte("00");
            if (VCI_Transmit(m_devtype, m_devind, m_canind, ref sendobj2, 1) == 0)
            {
                MessageBox.Show("发送失败", "错误",
                        MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
            System.Threading.Thread.Sleep(100);
            VCI_CAN_OBJ sendobj6 = new VCI_CAN_OBJ();
            sendobj6.ExternFlag = 1;
            sendobj6.ID = System.Convert.ToUInt32("18000029", 16);
            sendobj6.DataLen = 8;
            sendobj6.Data[0] = Convert.ToByte("191");
            sendobj6.Data[1] = Convert.ToByte("184");
            sendobj6.Data[2] = Convert.ToByte("00");
            sendobj6.Data[3] = Convert.ToByte("00");
            sendobj6.Data[4] = Convert.ToByte("00");
            sendobj6.Data[5] = Convert.ToByte("00");
            sendobj6.Data[6] = Convert.ToByte("00");
            sendobj6.Data[7] = Convert.ToByte("00");
            if (VCI_Transmit(m_devtype, m_devind, m_canind, ref sendobj6, 1) == 0)
            {
                MessageBox.Show("发送失败", "错误",
                        MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
            System.Threading.Thread.Sleep(1500);
            VCI_CAN_OBJ sendobj7 = new VCI_CAN_OBJ();
            sendobj7.ExternFlag = 1;
            sendobj7.ID = System.Convert.ToUInt32("18000029", 16);
            sendobj7.DataLen = 8;
            sendobj7.Data[0] = Convert.ToByte("191");
            sendobj7.Data[1] = Convert.ToByte("185");
            sendobj7.Data[2] = Convert.ToByte("00");
            sendobj7.Data[3] = Convert.ToByte("00");
            sendobj7.Data[4] = Convert.ToByte("00");
            sendobj7.Data[5] = Convert.ToByte("00");
            sendobj7.Data[6] = Convert.ToByte("00");
            sendobj7.Data[7] = Convert.ToByte("00");
            if (VCI_Transmit(m_devtype, m_devind, m_canind, ref sendobj7, 1) == 0)
            {
                MessageBox.Show("发送失败", "错误",
                        MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
            System.Threading.Thread.Sleep(1500);
            VCI_CAN_OBJ sendobj8 = new VCI_CAN_OBJ();
            sendobj8.ExternFlag = 1;
            sendobj8.ID = System.Convert.ToUInt32("18000029", 16);
            sendobj8.DataLen = 8;
            sendobj8.Data[0] = Convert.ToByte("191");
            sendobj8.Data[1] = Convert.ToByte("186");
            sendobj8.Data[2] = Convert.ToByte("00");
            sendobj8.Data[3] = Convert.ToByte("00");
            sendobj8.Data[4] = Convert.ToByte("00");
            sendobj8.Data[5] = Convert.ToByte("00");
            sendobj8.Data[6] = Convert.ToByte("00");
            sendobj8.Data[7] = Convert.ToByte("00");
            if (VCI_Transmit(m_devtype, m_devind, m_canind, ref sendobj8, 1) == 0)
            {
                MessageBox.Show("发送失败", "错误",
                        MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
            System.Threading.Thread.Sleep(1500);
            callback1.ID72 = temp[74]; callback1.ID73 = temp[75]; callback1.ID74 = temp[76];
            充电温度过高时间标定.Enabled = true;
        }
       unsafe private void 充电温度过低时间标定_Click(object sender, EventArgs e)
        {
            充电温度过低时间标定.Enabled = false;
            UInt32[] a = new UInt32[4];
            if (m_bOpen == 0)
                return;
            VCI_CAN_OBJ sendobj = new VCI_CAN_OBJ();
            sendobj.ExternFlag = 1;
            sendobj.ID = System.Convert.ToUInt32("18000029", 16);
            sendobj.DataLen = 8;
            sendobj.Data[0] = Convert.ToByte("127");
            sendobj.Data[1] = Convert.ToByte("187");
            sendobj.Data[2] = Convert.ToByte("00");
            sendobj.Data[3] = Convert.ToByte((Convert.ToDouble(充电温度过低时间1.Text)) * 10);
            sendobj.Data[4] = Convert.ToByte("00");
            sendobj.Data[5] = Convert.ToByte("00");
            sendobj.Data[6] = Convert.ToByte("00");
            sendobj.Data[7] = Convert.ToByte("00");
            if (VCI_Transmit(m_devtype, m_devind, m_canind, ref sendobj, 1) == 0)
            {
                MessageBox.Show("发送失败", "错误",
                        MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
            System.Threading.Thread.Sleep(100);

            VCI_CAN_OBJ sendobj1 = new VCI_CAN_OBJ();
            sendobj1.ExternFlag = 1;
            sendobj1.ID = System.Convert.ToUInt32("18000029", 16);
            sendobj1.DataLen = 8;
            sendobj1.Data[0] = Convert.ToByte("127");
            sendobj1.Data[1] = Convert.ToByte("188");
            sendobj1.Data[2] = Convert.ToByte("00");
            sendobj1.Data[3] = Convert.ToByte((Convert.ToDouble(充电温度过低时间2.Text)) * 10);
            sendobj1.Data[4] = Convert.ToByte("00");
            sendobj1.Data[5] = Convert.ToByte("00");
            sendobj1.Data[6] = Convert.ToByte("00");
            sendobj1.Data[7] = Convert.ToByte("00");
            if (VCI_Transmit(m_devtype, m_devind, m_canind, ref sendobj1, 1) == 0)
            {
                MessageBox.Show("发送失败", "错误",
                        MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
            System.Threading.Thread.Sleep(100);
            if (m_bOpen == 0)
                return;
            VCI_CAN_OBJ sendobj2 = new VCI_CAN_OBJ();
            sendobj2.ExternFlag = 1;
            sendobj2.ID = System.Convert.ToUInt32("18000029", 16);
            sendobj2.DataLen = 8;
            sendobj2.Data[0] = Convert.ToByte("127");
            sendobj2.Data[1] = Convert.ToByte("189");
            sendobj2.Data[2] = Convert.ToByte("00");
            sendobj2.Data[3] = Convert.ToByte((Convert.ToDouble(充电温度过低时间3.Text)) * 10);
            sendobj2.Data[4] = Convert.ToByte("00");
            sendobj2.Data[5] = Convert.ToByte("00");
            sendobj2.Data[6] = Convert.ToByte("00");
            sendobj2.Data[7] = Convert.ToByte("00");
            if (VCI_Transmit(m_devtype, m_devind, m_canind, ref sendobj2, 1) == 0)
            {
                MessageBox.Show("发送失败", "错误",
                        MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
            System.Threading.Thread.Sleep(100);
            VCI_CAN_OBJ sendobj6 = new VCI_CAN_OBJ();
            sendobj6.ExternFlag = 1;
            sendobj6.ID = System.Convert.ToUInt32("18000029", 16);
            sendobj6.DataLen = 8;
            sendobj6.Data[0] = Convert.ToByte("191");
            sendobj6.Data[1] = Convert.ToByte("187");
            sendobj6.Data[2] = Convert.ToByte("00");
            sendobj6.Data[3] = Convert.ToByte("00");
            sendobj6.Data[4] = Convert.ToByte("00");
            sendobj6.Data[5] = Convert.ToByte("00");
            sendobj6.Data[6] = Convert.ToByte("00");
            sendobj6.Data[7] = Convert.ToByte("00");
            if (VCI_Transmit(m_devtype, m_devind, m_canind, ref sendobj6, 1) == 0)
            {
                MessageBox.Show("发送失败", "错误",
                        MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
            System.Threading.Thread.Sleep(1500);
            VCI_CAN_OBJ sendobj7 = new VCI_CAN_OBJ();
            sendobj7.ExternFlag = 1;
            sendobj7.ID = System.Convert.ToUInt32("18000029", 16);
            sendobj7.DataLen = 8;
            sendobj7.Data[0] = Convert.ToByte("191");
            sendobj7.Data[1] = Convert.ToByte("188");
            sendobj7.Data[2] = Convert.ToByte("00");
            sendobj7.Data[3] = Convert.ToByte("00");
            sendobj7.Data[4] = Convert.ToByte("00");
            sendobj7.Data[5] = Convert.ToByte("00");
            sendobj7.Data[6] = Convert.ToByte("00");
            sendobj7.Data[7] = Convert.ToByte("00");
            if (VCI_Transmit(m_devtype, m_devind, m_canind, ref sendobj7, 1) == 0)
            {
                MessageBox.Show("发送失败", "错误",
                        MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
            System.Threading.Thread.Sleep(1500);
            VCI_CAN_OBJ sendobj8 = new VCI_CAN_OBJ();
            sendobj8.ExternFlag = 1;
            sendobj8.ID = System.Convert.ToUInt32("18000029", 16);
            sendobj8.DataLen = 8;
            sendobj8.Data[0] = Convert.ToByte("191");
            sendobj8.Data[1] = Convert.ToByte("189");
            sendobj8.Data[2] = Convert.ToByte("00");
            sendobj8.Data[3] = Convert.ToByte("00");
            sendobj8.Data[4] = Convert.ToByte("00");
            sendobj8.Data[5] = Convert.ToByte("00");
            sendobj8.Data[6] = Convert.ToByte("00");
            sendobj8.Data[7] = Convert.ToByte("00");
            if (VCI_Transmit(m_devtype, m_devind, m_canind, ref sendobj8, 1) == 0)
            {
                MessageBox.Show("发送失败", "错误",
                        MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
            System.Threading.Thread.Sleep(1500);
            callback1.ID75 = temp[77]; callback1.ID76 = temp[78]; callback1.ID77 = temp[79];
            充电温度过低时间标定.Enabled = true;
        }
        unsafe private void 放电温度过高时间标定_Click(object sender, EventArgs e)
        {
            放电温度过高时间标定.Enabled = false;
            UInt32[] a = new UInt32[4];
            if (m_bOpen == 0)
                return;
            VCI_CAN_OBJ sendobj = new VCI_CAN_OBJ();
            sendobj.ExternFlag = 1;
            sendobj.ID = System.Convert.ToUInt32("18000029", 16);
            sendobj.DataLen = 8;
            sendobj.Data[0] = Convert.ToByte("127");
            sendobj.Data[1] = Convert.ToByte("190");
            sendobj.Data[2] = Convert.ToByte("00");
            sendobj.Data[3] = Convert.ToByte((Convert.ToDouble(放电温度过高时间1.Text)) * 10);
            sendobj.Data[4] = Convert.ToByte("00");
            sendobj.Data[5] = Convert.ToByte("00");
            sendobj.Data[6] = Convert.ToByte("00");
            sendobj.Data[7] = Convert.ToByte("00");
            if (VCI_Transmit(m_devtype, m_devind, m_canind, ref sendobj, 1) == 0)
            {
                MessageBox.Show("发送失败", "错误",
                        MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
            System.Threading.Thread.Sleep(100);

            VCI_CAN_OBJ sendobj1 = new VCI_CAN_OBJ();
            sendobj1.ExternFlag = 1;
            sendobj1.ID = System.Convert.ToUInt32("18000029", 16);
            sendobj1.DataLen = 8;
            sendobj1.Data[0] = Convert.ToByte("127");
            sendobj1.Data[1] = Convert.ToByte("191");
            sendobj1.Data[2] = Convert.ToByte("00");
            sendobj1.Data[3] = Convert.ToByte((Convert.ToDouble(放电温度过高时间2.Text)) * 10);
            sendobj1.Data[4] = Convert.ToByte("00");
            sendobj1.Data[5] = Convert.ToByte("00");
            sendobj1.Data[6] = Convert.ToByte("00");
            sendobj1.Data[7] = Convert.ToByte("00");
            if (VCI_Transmit(m_devtype, m_devind, m_canind, ref sendobj1, 1) == 0)
            {
                MessageBox.Show("发送失败", "错误",
                        MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
            System.Threading.Thread.Sleep(100);
            if (m_bOpen == 0)
                return;
            VCI_CAN_OBJ sendobj2 = new VCI_CAN_OBJ();
            sendobj2.ExternFlag = 1;
            sendobj2.ID = System.Convert.ToUInt32("18000029", 16);
            sendobj2.DataLen = 8;
            sendobj2.Data[0] = Convert.ToByte("127");
            sendobj2.Data[1] = Convert.ToByte("192");
            sendobj2.Data[2] = Convert.ToByte("00");
            sendobj2.Data[3] = Convert.ToByte((Convert.ToDouble(放电温度过高时间3.Text)) * 10);
            sendobj2.Data[4] = Convert.ToByte("00");
            sendobj2.Data[5] = Convert.ToByte("00");
            sendobj2.Data[6] = Convert.ToByte("00");
            sendobj2.Data[7] = Convert.ToByte("00");
            if (VCI_Transmit(m_devtype, m_devind, m_canind, ref sendobj2, 1) == 0)
            {
                MessageBox.Show("发送失败", "错误",
                        MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
            System.Threading.Thread.Sleep(100);
            VCI_CAN_OBJ sendobj6 = new VCI_CAN_OBJ();
            sendobj6.ExternFlag = 1;
            sendobj6.ID = System.Convert.ToUInt32("18000029", 16);
            sendobj6.DataLen = 8;
            sendobj6.Data[0] = Convert.ToByte("191");
            sendobj6.Data[1] = Convert.ToByte("190");
            sendobj6.Data[2] = Convert.ToByte("00");
            sendobj6.Data[3] = Convert.ToByte("00");
            sendobj6.Data[4] = Convert.ToByte("00");
            sendobj6.Data[5] = Convert.ToByte("00");
            sendobj6.Data[6] = Convert.ToByte("00");
            sendobj6.Data[7] = Convert.ToByte("00");
            if (VCI_Transmit(m_devtype, m_devind, m_canind, ref sendobj6, 1) == 0)
            {
                MessageBox.Show("发送失败", "错误",
                        MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
            System.Threading.Thread.Sleep(1500);
            VCI_CAN_OBJ sendobj7 = new VCI_CAN_OBJ();
            sendobj7.ExternFlag = 1;
            sendobj7.ID = System.Convert.ToUInt32("18000029", 16);
            sendobj7.DataLen = 8;
            sendobj7.Data[0] = Convert.ToByte("191");
            sendobj7.Data[1] = Convert.ToByte("191");
            sendobj7.Data[2] = Convert.ToByte("00");
            sendobj7.Data[3] = Convert.ToByte("00");
            sendobj7.Data[4] = Convert.ToByte("00");
            sendobj7.Data[5] = Convert.ToByte("00");
            sendobj7.Data[6] = Convert.ToByte("00");
            sendobj7.Data[7] = Convert.ToByte("00");
            if (VCI_Transmit(m_devtype, m_devind, m_canind, ref sendobj7, 1) == 0)
            {
                MessageBox.Show("发送失败", "错误",
                        MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
            System.Threading.Thread.Sleep(1500);
            VCI_CAN_OBJ sendobj8 = new VCI_CAN_OBJ();
            sendobj8.ExternFlag = 1;
            sendobj8.ID = System.Convert.ToUInt32("18000029", 16);
            sendobj8.DataLen = 8;
            sendobj8.Data[0] = Convert.ToByte("191");
            sendobj8.Data[1] = Convert.ToByte("192");
            sendobj8.Data[2] = Convert.ToByte("00");
            sendobj8.Data[3] = Convert.ToByte("00");
            sendobj8.Data[4] = Convert.ToByte("00");
            sendobj8.Data[5] = Convert.ToByte("00");
            sendobj8.Data[6] = Convert.ToByte("00");
            sendobj8.Data[7] = Convert.ToByte("00");
            if (VCI_Transmit(m_devtype, m_devind, m_canind, ref sendobj8, 1) == 0)
            {
                MessageBox.Show("发送失败", "错误",
                        MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
            System.Threading.Thread.Sleep(1500);
            callback1.ID78 = temp[80]; callback1.ID79 = temp[81]; callback1.ID80 = temp[82];
            放电温度过高时间标定.Enabled = true;
        }
        unsafe private void 放电温度过低时间标定_Click(object sender, EventArgs e)
        {
            放电温度过低时间标定.Enabled = false;
            UInt32[] a = new UInt32[4];
            if (m_bOpen == 0)
                return;
            VCI_CAN_OBJ sendobj = new VCI_CAN_OBJ();
            sendobj.ExternFlag = 1;
            sendobj.ID = System.Convert.ToUInt32("18000029", 16);
            sendobj.DataLen = 8;
            sendobj.Data[0] = Convert.ToByte("127");
            sendobj.Data[1] = Convert.ToByte("193");
            sendobj.Data[2] = Convert.ToByte("00");
            sendobj.Data[3] = Convert.ToByte((Convert.ToDouble(放电温度过低时间1.Text)) * 10);
            sendobj.Data[4] = Convert.ToByte("00");
            sendobj.Data[5] = Convert.ToByte("00");
            sendobj.Data[6] = Convert.ToByte("00");
            sendobj.Data[7] = Convert.ToByte("00");
            if (VCI_Transmit(m_devtype, m_devind, m_canind, ref sendobj, 1) == 0)
            {
                MessageBox.Show("发送失败", "错误",
                        MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
            System.Threading.Thread.Sleep(100);

            VCI_CAN_OBJ sendobj1 = new VCI_CAN_OBJ();
            sendobj1.ExternFlag = 1;
            sendobj1.ID = System.Convert.ToUInt32("18000029", 16);
            sendobj1.DataLen = 8;
            sendobj1.Data[0] = Convert.ToByte("127");
            sendobj1.Data[1] = Convert.ToByte("194");
            sendobj1.Data[2] = Convert.ToByte("00");
            sendobj1.Data[3] = Convert.ToByte((Convert.ToDouble(放电温度过低时间2.Text)) * 10);
            sendobj1.Data[4] = Convert.ToByte("00");
            sendobj1.Data[5] = Convert.ToByte("00");
            sendobj1.Data[6] = Convert.ToByte("00");
            sendobj1.Data[7] = Convert.ToByte("00");
            if (VCI_Transmit(m_devtype, m_devind, m_canind, ref sendobj1, 1) == 0)
            {
                MessageBox.Show("发送失败", "错误",
                        MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
            System.Threading.Thread.Sleep(100);
            if (m_bOpen == 0)
                return;
            VCI_CAN_OBJ sendobj2 = new VCI_CAN_OBJ();
            sendobj2.ExternFlag = 1;
            sendobj2.ID = System.Convert.ToUInt32("18000029", 16);
            sendobj2.DataLen = 8;
            sendobj2.Data[0] = Convert.ToByte("127");
            sendobj2.Data[1] = Convert.ToByte("195");
            sendobj2.Data[2] = Convert.ToByte("00");
            sendobj2.Data[3] = Convert.ToByte((Convert.ToDouble(放电温度过低时间3.Text)) * 10);
            sendobj2.Data[4] = Convert.ToByte("00");
            sendobj2.Data[5] = Convert.ToByte("00");
            sendobj2.Data[6] = Convert.ToByte("00");
            sendobj2.Data[7] = Convert.ToByte("00");
            if (VCI_Transmit(m_devtype, m_devind, m_canind, ref sendobj2, 1) == 0)
            {
                MessageBox.Show("发送失败", "错误",
                        MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
            System.Threading.Thread.Sleep(100);
            VCI_CAN_OBJ sendobj6 = new VCI_CAN_OBJ();
            sendobj6.ExternFlag = 1;
            sendobj6.ID = System.Convert.ToUInt32("18000029", 16);
            sendobj6.DataLen = 8;
            sendobj6.Data[0] = Convert.ToByte("191");
            sendobj6.Data[1] = Convert.ToByte("193");
            sendobj6.Data[2] = Convert.ToByte("00");
            sendobj6.Data[3] = Convert.ToByte("00");
            sendobj6.Data[4] = Convert.ToByte("00");
            sendobj6.Data[5] = Convert.ToByte("00");
            sendobj6.Data[6] = Convert.ToByte("00");
            sendobj6.Data[7] = Convert.ToByte("00");
            if (VCI_Transmit(m_devtype, m_devind, m_canind, ref sendobj6, 1) == 0)
            {
                MessageBox.Show("发送失败", "错误",
                        MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
            System.Threading.Thread.Sleep(1500);
            VCI_CAN_OBJ sendobj7 = new VCI_CAN_OBJ();
            sendobj7.ExternFlag = 1;
            sendobj7.ID = System.Convert.ToUInt32("18000029", 16);
            sendobj7.DataLen = 8;
            sendobj7.Data[0] = Convert.ToByte("191");
            sendobj7.Data[1] = Convert.ToByte("194");
            sendobj7.Data[2] = Convert.ToByte("00");
            sendobj7.Data[3] = Convert.ToByte("00");
            sendobj7.Data[4] = Convert.ToByte("00");
            sendobj7.Data[5] = Convert.ToByte("00");
            sendobj7.Data[6] = Convert.ToByte("00");
            sendobj7.Data[7] = Convert.ToByte("00");
            if (VCI_Transmit(m_devtype, m_devind, m_canind, ref sendobj7, 1) == 0)
            {
                MessageBox.Show("发送失败", "错误",
                        MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
            System.Threading.Thread.Sleep(1500);
            VCI_CAN_OBJ sendobj8 = new VCI_CAN_OBJ();
            sendobj8.ExternFlag = 1;
            sendobj8.ID = System.Convert.ToUInt32("18000029", 16);
            sendobj8.DataLen = 8;
            sendobj8.Data[0] = Convert.ToByte("191");
            sendobj8.Data[1] = Convert.ToByte("195");
            sendobj8.Data[2] = Convert.ToByte("00");
            sendobj8.Data[3] = Convert.ToByte("00");
            sendobj8.Data[4] = Convert.ToByte("00");
            sendobj8.Data[5] = Convert.ToByte("00");
            sendobj8.Data[6] = Convert.ToByte("00");
            sendobj8.Data[7] = Convert.ToByte("00");
            if (VCI_Transmit(m_devtype, m_devind, m_canind, ref sendobj8, 1) == 0)
            {
                MessageBox.Show("发送失败", "错误",
                        MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
            System.Threading.Thread.Sleep(1500);
            callback1.ID81 = temp[83]; callback1.ID82 = temp[84]; callback1.ID83 = temp[85];
            放电温度过低时间标定.Enabled = true;
        }
        unsafe private void 单体压差过大时间标定_Click(object sender, EventArgs e)
        {
            单体压差过大时间标定.Enabled = false;
            UInt32[] a = new UInt32[4];
            if (m_bOpen == 0)
                return;
            VCI_CAN_OBJ sendobj = new VCI_CAN_OBJ();
            sendobj.ExternFlag = 1;
            sendobj.ID = System.Convert.ToUInt32("18000029", 16);
            sendobj.DataLen = 8;
            sendobj.Data[0] = Convert.ToByte("127");
            sendobj.Data[1] = Convert.ToByte("196");
            sendobj.Data[2] = Convert.ToByte("00");
            sendobj.Data[3] = Convert.ToByte((Convert.ToDouble(单体压差过大时间1.Text)) * 10);
            sendobj.Data[4] = Convert.ToByte("00");
            sendobj.Data[5] = Convert.ToByte("00");
            sendobj.Data[6] = Convert.ToByte("00");
            sendobj.Data[7] = Convert.ToByte("00");
            if (VCI_Transmit(m_devtype, m_devind, m_canind, ref sendobj, 1) == 0)
            {
                MessageBox.Show("发送失败", "错误",
                        MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
            System.Threading.Thread.Sleep(100);

            VCI_CAN_OBJ sendobj1 = new VCI_CAN_OBJ();
            sendobj1.ExternFlag = 1;
            sendobj1.ID = System.Convert.ToUInt32("18000029", 16);
            sendobj1.DataLen = 8;
            sendobj1.Data[0] = Convert.ToByte("127");
            sendobj1.Data[1] = Convert.ToByte("197");
            sendobj1.Data[2] = Convert.ToByte("00");
            sendobj1.Data[3] = Convert.ToByte((Convert.ToDouble(单体压差过大时间2.Text)) * 10);
            sendobj1.Data[4] = Convert.ToByte("00");
            sendobj1.Data[5] = Convert.ToByte("00");
            sendobj1.Data[6] = Convert.ToByte("00");
            sendobj1.Data[7] = Convert.ToByte("00");
            if (VCI_Transmit(m_devtype, m_devind, m_canind, ref sendobj1, 1) == 0)
            {
                MessageBox.Show("发送失败", "错误",
                        MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
            System.Threading.Thread.Sleep(100);
            if (m_bOpen == 0)
                return;
            VCI_CAN_OBJ sendobj2 = new VCI_CAN_OBJ();
            sendobj2.ExternFlag = 1;
            sendobj2.ID = System.Convert.ToUInt32("18000029", 16);
            sendobj2.DataLen = 8;
            sendobj2.Data[0] = Convert.ToByte("127");
            sendobj2.Data[1] = Convert.ToByte("198");
            sendobj2.Data[2] = Convert.ToByte("00");
            sendobj2.Data[3] = Convert.ToByte((Convert.ToDouble(单体压差过大时间3.Text)) * 10);
            sendobj2.Data[4] = Convert.ToByte("00");
            sendobj2.Data[5] = Convert.ToByte("00");
            sendobj2.Data[6] = Convert.ToByte("00");
            sendobj2.Data[7] = Convert.ToByte("00");
            if (VCI_Transmit(m_devtype, m_devind, m_canind, ref sendobj2, 1) == 0)
            {
                MessageBox.Show("发送失败", "错误",
                        MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
            System.Threading.Thread.Sleep(100);
            VCI_CAN_OBJ sendobj6 = new VCI_CAN_OBJ();
            sendobj6.ExternFlag = 1;
            sendobj6.ID = System.Convert.ToUInt32("18000029", 16);
            sendobj6.DataLen = 8;
            sendobj6.Data[0] = Convert.ToByte("191");
            sendobj6.Data[1] = Convert.ToByte("196");
            sendobj6.Data[2] = Convert.ToByte("00");
            sendobj6.Data[3] = Convert.ToByte("00");
            sendobj6.Data[4] = Convert.ToByte("00");
            sendobj6.Data[5] = Convert.ToByte("00");
            sendobj6.Data[6] = Convert.ToByte("00");
            sendobj6.Data[7] = Convert.ToByte("00");
            if (VCI_Transmit(m_devtype, m_devind, m_canind, ref sendobj6, 1) == 0)
            {
                MessageBox.Show("发送失败", "错误",
                        MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
            System.Threading.Thread.Sleep(1500);
            VCI_CAN_OBJ sendobj7 = new VCI_CAN_OBJ();
            sendobj7.ExternFlag = 1;
            sendobj7.ID = System.Convert.ToUInt32("18000029", 16);
            sendobj7.DataLen = 8;
            sendobj7.Data[0] = Convert.ToByte("191");
            sendobj7.Data[1] = Convert.ToByte("197");
            sendobj7.Data[2] = Convert.ToByte("00");
            sendobj7.Data[3] = Convert.ToByte("00");
            sendobj7.Data[4] = Convert.ToByte("00");
            sendobj7.Data[5] = Convert.ToByte("00");
            sendobj7.Data[6] = Convert.ToByte("00");
            sendobj7.Data[7] = Convert.ToByte("00");
            if (VCI_Transmit(m_devtype, m_devind, m_canind, ref sendobj7, 1) == 0)
            {
                MessageBox.Show("发送失败", "错误",
                        MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
            System.Threading.Thread.Sleep(1500);
            VCI_CAN_OBJ sendobj8 = new VCI_CAN_OBJ();
            sendobj8.ExternFlag = 1;
            sendobj8.ID = System.Convert.ToUInt32("18000029", 16);
            sendobj8.DataLen = 8;
            sendobj8.Data[0] = Convert.ToByte("191");
            sendobj8.Data[1] = Convert.ToByte("198");
            sendobj8.Data[2] = Convert.ToByte("00");
            sendobj8.Data[3] = Convert.ToByte("00");
            sendobj8.Data[4] = Convert.ToByte("00");
            sendobj8.Data[5] = Convert.ToByte("00");
            sendobj8.Data[6] = Convert.ToByte("00");
            sendobj8.Data[7] = Convert.ToByte("00");
            if (VCI_Transmit(m_devtype, m_devind, m_canind, ref sendobj8, 1) == 0)
            {
                MessageBox.Show("发送失败", "错误",
                        MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
            System.Threading.Thread.Sleep(1500);
            callback1.ID84 = temp[86]; callback1.ID85 = temp[87]; callback1.ID86 = temp[88];
            单体压差过大时间标定.Enabled = true;
        }
        unsafe private void 温差过大时间标定_Click(object sender, EventArgs e)
        {
            温差过大时间标定.Enabled = false;
            UInt32[] a = new UInt32[4];
            if (m_bOpen == 0)
                return;
            VCI_CAN_OBJ sendobj = new VCI_CAN_OBJ();
            sendobj.ExternFlag = 1;
            sendobj.ID = System.Convert.ToUInt32("18000029", 16);
            sendobj.DataLen = 8;
            sendobj.Data[0] = Convert.ToByte("127");
            sendobj.Data[1] = Convert.ToByte("199");
            sendobj.Data[2] = Convert.ToByte("00");
            sendobj.Data[3] = Convert.ToByte((Convert.ToDouble(温差过大时间1.Text)) * 10);
            sendobj.Data[4] = Convert.ToByte("00");
            sendobj.Data[5] = Convert.ToByte("00");
            sendobj.Data[6] = Convert.ToByte("00");
            sendobj.Data[7] = Convert.ToByte("00");
            if (VCI_Transmit(m_devtype, m_devind, m_canind, ref sendobj, 1) == 0)
            {
                MessageBox.Show("发送失败", "错误",
                        MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
            System.Threading.Thread.Sleep(100);

            VCI_CAN_OBJ sendobj1 = new VCI_CAN_OBJ();
            sendobj1.ExternFlag = 1;
            sendobj1.ID = System.Convert.ToUInt32("18000029", 16);
            sendobj1.DataLen = 8;
            sendobj1.Data[0] = Convert.ToByte("127");
            sendobj1.Data[1] = Convert.ToByte("200");
            sendobj1.Data[2] = Convert.ToByte("00");
            sendobj1.Data[3] = Convert.ToByte((Convert.ToDouble(温差过大时间2.Text)) * 10);
            sendobj1.Data[4] = Convert.ToByte("00");
            sendobj1.Data[5] = Convert.ToByte("00");
            sendobj1.Data[6] = Convert.ToByte("00");
            sendobj1.Data[7] = Convert.ToByte("00");
            if (VCI_Transmit(m_devtype, m_devind, m_canind, ref sendobj1, 1) == 0)
            {
                MessageBox.Show("发送失败", "错误",
                        MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
            System.Threading.Thread.Sleep(100);
            if (m_bOpen == 0)
                return;
            VCI_CAN_OBJ sendobj2 = new VCI_CAN_OBJ();
            sendobj2.ExternFlag = 1;
            sendobj2.ID = System.Convert.ToUInt32("18000029", 16);
            sendobj2.DataLen = 8;
            sendobj2.Data[0] = Convert.ToByte("127");
            sendobj2.Data[1] = Convert.ToByte("201");
            sendobj2.Data[2] = Convert.ToByte("00");
            sendobj2.Data[3] = Convert.ToByte((Convert.ToDouble(温差过大时间3.Text)) * 10);
            sendobj2.Data[4] = Convert.ToByte("00");
            sendobj2.Data[5] = Convert.ToByte("00");
            sendobj2.Data[6] = Convert.ToByte("00");
            sendobj2.Data[7] = Convert.ToByte("00");
            if (VCI_Transmit(m_devtype, m_devind, m_canind, ref sendobj2, 1) == 0)
            {
                MessageBox.Show("发送失败", "错误",
                        MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
            System.Threading.Thread.Sleep(100);
            VCI_CAN_OBJ sendobj6 = new VCI_CAN_OBJ();
            sendobj6.ExternFlag = 1;
            sendobj6.ID = System.Convert.ToUInt32("18000029", 16);
            sendobj6.DataLen = 8;
            sendobj6.Data[0] = Convert.ToByte("191");
            sendobj6.Data[1] = Convert.ToByte("199");
            sendobj6.Data[2] = Convert.ToByte("00");
            sendobj6.Data[3] = Convert.ToByte("00");
            sendobj6.Data[4] = Convert.ToByte("00");
            sendobj6.Data[5] = Convert.ToByte("00");
            sendobj6.Data[6] = Convert.ToByte("00");
            sendobj6.Data[7] = Convert.ToByte("00");
            if (VCI_Transmit(m_devtype, m_devind, m_canind, ref sendobj6, 1) == 0)
            {
                MessageBox.Show("发送失败", "错误",
                        MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
            System.Threading.Thread.Sleep(1500);
            VCI_CAN_OBJ sendobj7 = new VCI_CAN_OBJ();
            sendobj7.ExternFlag = 1;
            sendobj7.ID = System.Convert.ToUInt32("18000029", 16);
            sendobj7.DataLen = 8;
            sendobj7.Data[0] = Convert.ToByte("191");
            sendobj7.Data[1] = Convert.ToByte("200");
            sendobj7.Data[2] = Convert.ToByte("00");
            sendobj7.Data[3] = Convert.ToByte("00");
            sendobj7.Data[4] = Convert.ToByte("00");
            sendobj7.Data[5] = Convert.ToByte("00");
            sendobj7.Data[6] = Convert.ToByte("00");
            sendobj7.Data[7] = Convert.ToByte("00");
            if (VCI_Transmit(m_devtype, m_devind, m_canind, ref sendobj7, 1) == 0)
            {
                MessageBox.Show("发送失败", "错误",
                        MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
            System.Threading.Thread.Sleep(1500);
            VCI_CAN_OBJ sendobj8 = new VCI_CAN_OBJ();
            sendobj8.ExternFlag = 1;
            sendobj8.ID = System.Convert.ToUInt32("18000029", 16);
            sendobj8.DataLen = 8;
            sendobj8.Data[0] = Convert.ToByte("191");
            sendobj8.Data[1] = Convert.ToByte("201");
            sendobj8.Data[2] = Convert.ToByte("00");
            sendobj8.Data[3] = Convert.ToByte("00");
            sendobj8.Data[4] = Convert.ToByte("00");
            sendobj8.Data[5] = Convert.ToByte("00");
            sendobj8.Data[6] = Convert.ToByte("00");
            sendobj8.Data[7] = Convert.ToByte("00");
            if (VCI_Transmit(m_devtype, m_devind, m_canind, ref sendobj8, 1) == 0)
            {
                MessageBox.Show("发送失败", "错误",
                        MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
            System.Threading.Thread.Sleep(1500);
            callback1.ID87 = temp[89]; callback1.ID88 = temp[90]; callback1.ID89= temp[91];
            温差过大时间标定.Enabled = true;

        }
        unsafe private void SOC过低时间标定_Click(object sender, EventArgs e)
        {
            SOC过低时间标定.Enabled = false;
            UInt32[] a = new UInt32[4];
            if (m_bOpen == 0)
                return;
            VCI_CAN_OBJ sendobj = new VCI_CAN_OBJ();
            sendobj.ExternFlag = 1;
            sendobj.ID = System.Convert.ToUInt32("18000029", 16);
            sendobj.DataLen = 8;
            sendobj.Data[0] = Convert.ToByte("127");
            sendobj.Data[1] = Convert.ToByte("202");
            sendobj.Data[2] = Convert.ToByte("00");
            sendobj.Data[3] = Convert.ToByte((Convert.ToDouble(SOC过低时间1.Text)) * 10);
            sendobj.Data[4] = Convert.ToByte("00");
            sendobj.Data[5] = Convert.ToByte("00");
            sendobj.Data[6] = Convert.ToByte("00");
            sendobj.Data[7] = Convert.ToByte("00");
            if (VCI_Transmit(m_devtype, m_devind, m_canind, ref sendobj, 1) == 0)
            {
                MessageBox.Show("发送失败", "错误",
                        MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
            System.Threading.Thread.Sleep(100);

            VCI_CAN_OBJ sendobj1 = new VCI_CAN_OBJ();
            sendobj1.ExternFlag = 1;
            sendobj1.ID = System.Convert.ToUInt32("18000029", 16);
            sendobj1.DataLen = 8;
            sendobj1.Data[0] = Convert.ToByte("127");
            sendobj1.Data[1] = Convert.ToByte("203");
            sendobj1.Data[2] = Convert.ToByte("00");
            sendobj1.Data[3] = Convert.ToByte((Convert.ToDouble(SOC过低时间2.Text)) * 10);
            sendobj1.Data[4] = Convert.ToByte("00");
            sendobj1.Data[5] = Convert.ToByte("00");
            sendobj1.Data[6] = Convert.ToByte("00");
            sendobj1.Data[7] = Convert.ToByte("00");
            if (VCI_Transmit(m_devtype, m_devind, m_canind, ref sendobj1, 1) == 0)
            {
                MessageBox.Show("发送失败", "错误",
                        MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
            System.Threading.Thread.Sleep(100);
            if (m_bOpen == 0)
                return;
            VCI_CAN_OBJ sendobj2 = new VCI_CAN_OBJ();
            sendobj2.ExternFlag = 1;
            sendobj2.ID = System.Convert.ToUInt32("18000029", 16);
            sendobj2.DataLen = 8;
            sendobj2.Data[0] = Convert.ToByte("127");
            sendobj2.Data[1] = Convert.ToByte("204");
            sendobj2.Data[2] = Convert.ToByte("00");
            sendobj2.Data[3] = Convert.ToByte((Convert.ToDouble(SOC过低时间3.Text)) * 10);
            sendobj2.Data[4] = Convert.ToByte("00");
            sendobj2.Data[5] = Convert.ToByte("00");
            sendobj2.Data[6] = Convert.ToByte("00");
            sendobj2.Data[7] = Convert.ToByte("00");
            if (VCI_Transmit(m_devtype, m_devind, m_canind, ref sendobj2, 1) == 0)
            {
                MessageBox.Show("发送失败", "错误",
                        MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
            System.Threading.Thread.Sleep(100);
            VCI_CAN_OBJ sendobj6 = new VCI_CAN_OBJ();
            sendobj6.ExternFlag = 1;
            sendobj6.ID = System.Convert.ToUInt32("18000029", 16);
            sendobj6.DataLen = 8;
            sendobj6.Data[0] = Convert.ToByte("191");
            sendobj6.Data[1] = Convert.ToByte("202");
            sendobj6.Data[2] = Convert.ToByte("00");
            sendobj6.Data[3] = Convert.ToByte("00");
            sendobj6.Data[4] = Convert.ToByte("00");
            sendobj6.Data[5] = Convert.ToByte("00");
            sendobj6.Data[6] = Convert.ToByte("00");
            sendobj6.Data[7] = Convert.ToByte("00");
            if (VCI_Transmit(m_devtype, m_devind, m_canind, ref sendobj6, 1) == 0)
            {
                MessageBox.Show("发送失败", "错误",
                        MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
            System.Threading.Thread.Sleep(1500);
            VCI_CAN_OBJ sendobj7 = new VCI_CAN_OBJ();
            sendobj7.ExternFlag = 1;
            sendobj7.ID = System.Convert.ToUInt32("18000029", 16);
            sendobj7.DataLen = 8;
            sendobj7.Data[0] = Convert.ToByte("191");
            sendobj7.Data[1] = Convert.ToByte("203");
            sendobj7.Data[2] = Convert.ToByte("00");
            sendobj7.Data[3] = Convert.ToByte("00");
            sendobj7.Data[4] = Convert.ToByte("00");
            sendobj7.Data[5] = Convert.ToByte("00");
            sendobj7.Data[6] = Convert.ToByte("00");
            sendobj7.Data[7] = Convert.ToByte("00");
            if (VCI_Transmit(m_devtype, m_devind, m_canind, ref sendobj7, 1) == 0)
            {
                MessageBox.Show("发送失败", "错误",
                        MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
            System.Threading.Thread.Sleep(1500);
            VCI_CAN_OBJ sendobj8 = new VCI_CAN_OBJ();
            sendobj8.ExternFlag = 1;
            sendobj8.ID = System.Convert.ToUInt32("18000029", 16);
            sendobj8.DataLen = 8;
            sendobj8.Data[0] = Convert.ToByte("191");
            sendobj8.Data[1] = Convert.ToByte("204");
            sendobj8.Data[2] = Convert.ToByte("00");
            sendobj8.Data[3] = Convert.ToByte("00");
            sendobj8.Data[4] = Convert.ToByte("00");
            sendobj8.Data[5] = Convert.ToByte("00");
            sendobj8.Data[6] = Convert.ToByte("00");
            sendobj8.Data[7] = Convert.ToByte("00");
            if (VCI_Transmit(m_devtype, m_devind, m_canind, ref sendobj8, 1) == 0)
            {
                MessageBox.Show("发送失败", "错误",
                        MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
            System.Threading.Thread.Sleep(1500);
            callback1.ID90= temp[92]; callback1.ID91 = temp[93]; callback1.ID92 = temp[94];
            SOC过低时间标定.Enabled = true;
        }
        #endregion
        public void canlist()
        {
            if (实时报文list.InvokeRequired)
            {
                //while (!实时报文list.IsHandleCreated)
                //{
                //    if (实时报文list.Disposing || 实时报文list.IsDisposed)
                //        return;
                //}
                changecanlist a = new changecanlist(showcanlist);
                实时报文list.BeginInvoke(a, new object[] {  });
            }
    }
        private void showcanlist()
        {
            实时报文list.Items.Add(canstr);
            实时报文list.SelectedIndex = 实时报文list.Items.Count - 1;
        }

        private void 连接pb1_Click(object sender, EventArgs e)
        {
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            System.Environment.Exit(0);
        }

        private void 存储1pic_Click_1(object sender, EventArgs e)
        {
                StreamWriter sw = new StreamWriter(System.Environment.CurrentDirectory + "\\MonitorData" + "\\" + RecStartTime + ".txt", true);
                for (int i = 0; i < 实时报文list.Items.Count; i++)
                {
                    sw.Write(实时报文list.Items[i]);
                    sw.WriteLine();
                }
                sw.Close();
            MessageBox.Show("导出完成！", "完成" ,MessageBoxButtons.OK, MessageBoxIcon.None);
        }

        private void 清除_Click(object sender, EventArgs e)
        {
            实时报文list.Items.Clear();
        }

      
    }
}
