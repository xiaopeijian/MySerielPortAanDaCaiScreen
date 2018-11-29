using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PortChat
{
    //该集合可以装用户集合或列表集合
    class PatientsAndProtocolsList
    {
        public string strPatientNumOrProtocolNum;
        public string strPatientNameOrProtocolName;
        public string strPatientSex;
        public string strPatientAge;
        public string strPatientBody;
        public string strStudyDateTime;
        public string strExposeValue;
        public string StrPatientNumOrProtocolNum
        {
            get
            {
                return strPatientNumOrProtocolNum;
            }
            set
            {
                strPatientNumOrProtocolNum = value;
            }
        }

        public string StrPatientNameOrProtocolName
        {
            get
            {
                return strPatientNameOrProtocolName;
            }
            set
            {
                strPatientNameOrProtocolName = value;
            }
        }

        public string StrPatientSex
        {
            get
            {
                return strPatientSex;
            }
            set
            {
                strPatientSex = value;
            }
        }

        public string StrPatientAge
        {
            get
            {
                return strPatientAge;
            }
            set
            {
                strPatientAge = value;
            }
        }

        public string StrPatientBody
        {
            get
            {
                return strPatientBody;
            }
            set
            {
                strPatientBody = value;
            }
        }

        public string StrStudyDateTime
        {
            get
            {
                return strStudyDateTime;
            }
            set
            {
                strStudyDateTime = value;
            }
        }
        
        public string StrExposeValue
        {
            get
            {
                return strExposeValue;
            }
            set
            {
                strExposeValue = value;
            }
        }


        public PatientsAndProtocolsList() { }
        public PatientsAndProtocolsList(string strExposeValue)
        {
            this.strExposeValue = strExposeValue;

        }

        /// <summary>
        /// 保存协议信息
        /// </summary>
        /// <param name="strProtocolNum"></param>
        /// <param name="strProtocolName"></param>
        public PatientsAndProtocolsList(string strProtocolNum, string strProtocolName)
        {
            this.strPatientNumOrProtocolNum = strProtocolNum;
            this.strPatientNameOrProtocolName = strProtocolName;
        }
        /// <summary>
        /// 保存病人信息：5个参数
        /// </summary>
        /// <param name="strPatientNum"></param>
        /// <param name="strPatientName"></param>
        /// <param name="strPatientSex"></param>
        /// <param name="strPatientAge"></param>
        /// <param name="strPatientBody"></param>
        public PatientsAndProtocolsList(string strPatientNum, string strPatientName, string strPatientSex, string strPatientAge, string strPatientBody)
        {
            this.strPatientNumOrProtocolNum = strPatientNum;
            this.strPatientNameOrProtocolName = strPatientName;
            this.strPatientSex = strPatientSex;
            this.strPatientAge = strPatientAge;
            this.strPatientBody = strPatientBody;
        }
        /// <summary>
        /// 保存病人信息：6个参数
        /// </summary>
        /// <param name="strPatientNum"></param>
        /// <param name="strPatientName"></param>
        /// <param name="strPatientSex"></param>
        /// <param name="strPatientAge"></param>
        /// <param name="strPatientBody"></param>
        /// <param name="studyDateTime"></param>
        public PatientsAndProtocolsList(string strPatientNum, string strPatientName, string strPatientSex, string strPatientAge, string strPatientBody, string studyDateTime)
        {
            this.strPatientNumOrProtocolNum = strPatientNum;
            this.strPatientNameOrProtocolName = strPatientName;
            this.strPatientSex = strPatientSex;
            this.strPatientAge = strPatientAge;
            this.strPatientBody = strPatientBody;
            this.strStudyDateTime = studyDateTime;
        }
    }
}
