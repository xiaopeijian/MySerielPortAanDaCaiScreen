using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PortChat
{
    //class GenStructData
    //{

    //}

    public struct ModeParamType
    {
        public int DeviceModeType;    //1-DR   2-RF
        public int BodySizeTyle;      //0-胖   1-中   2-瘦  3-小孩
        public int StandLayTyle;      //0-站位 1-躺位
    }

    public struct ExpRFGenParam         //RF剂量参数结构体
    {
        public string RfRisCode        //RF协议编号
        {
            set;
            get;
        }

        public string RfBodyPartName    //协议名称
        {
            set;
            get;
        }

        public int RfBodySID;           //SID值
        public int RfMatorCX;           //束光器宽度
        public int RfMatorCY;           //束光器高度
        public int RfKv;                //透视Kv值
        public float RfMa;              //透视Ma值
        public int RFSpot3PointKv;      //MAMS模式下点片KV值
        public float RFSpot3PointMa;    //MAMS模式下点片MA值
        public float RFSpot3PointMs;    //MAMS模式下点片MS值
        public int RFSpotAecKv;         //AEC模式下点片KV值
        public float RFSpotAecMa;       //AEC模式下点片MA值
        public float RFSpotAecMs;       //AEC模式下点片MS值
        public string RfProcParam;      //RF算法图像处理名称
        public string RfSpotProcParam;  //RF点片算法处理名称
        public int RfFrequency;         //使用频率
    }

    public struct ExpDRGenParam  //DR剂量参数结构体
    {
        public int TwoPoint_Kv;    //MAS模式Kv
        public float TwoPoint_Mas; //MAS模式Mas

        public int ThreePoint_Kv;  //MAMS模式Kv
        public float ThreePoint_Ma;//MAMS模式Ma
        public float ThreePoint_Ms;//MAMS模式Ms

        public int Aec_Kv;         //AEC模式Kv
        public float Aec_Ma;       //AEC模式Ma
        public float Aec_Ms;       //AEC模式Ms
        public int Aec_Den;        //AEC模式密度
        public int Aec_Fs;         //AEC模式屏速
        public int Aec_FieldL;     //AEC模式左视窗
        public int Aec_FieldC;     //AEC模式中视窗
        public int Aec_FieldR;     //AEC模式右视窗

        public int FlipH;          //垂直翻转
        public int FlipV;          //水平翻转
        public int Rotate;         //旋转

        public string SpotProcParam;//点片算法处理名称
        public string ProcParam;    //DR算法处理名称
        public int MatorCX;         //束光器宽度
        public int MatorCY;         //束光器高度

        public int ExposureMode;    //曝光模式 0-mAms 1-mAs 2-AEC
    }

    public struct DRGenParam
    {
        public ExpDRGenParam ChildGenParam;  //小孩剂量参数
        public ExpDRGenParam MediumGenParam; //中等身材型剂量参数
        public ExpDRGenParam FatGenParam;    //肥胖型剂量参数
        public ExpDRGenParam ThinGenParam;   //瘦型剂量参数
    }

    public struct ChoosableProtoInfo
    {
        public string ChoosableProtoNum  //可选协议编号
        {
            set;
            get;
        }

        public string ChoosableProtoName //可选协议名称
        {
            set;
            get;
        }

        public int SID;                //SID值
        public int Frequency;          //使用频率
    }

    public struct SelectedProto
    {
        public string CheckType         //检查部位类型  DR   RF
        {
            set;
            get;
        }

        public string ProtoName        //检查部位名称
        {
            set;
            get;
        }

        public string FinishStatue     //完成状态
        {
            set;
            get;
        }

        public string ProtoNum;    //协议编号
        public int SID;
        public int StudyFlag;      //检查标志 0-未检查  1-已检查
        public DRGenParam StanceParam; //站位
        public DRGenParam LayParam;    //躺位
        public DRGenParam M1Param;     //1米1位
        public DRGenParam M5Param;     //1米5位
        public DRGenParam M8Param;     //1米8位
        public ExpRFGenParam RFSelProto;
    }
}
