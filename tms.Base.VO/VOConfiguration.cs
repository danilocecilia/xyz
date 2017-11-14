using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace tms.Base.VO
{
    [Serializable]
    public class VOConfiguration
    {
        #region Properties

        public int ID { get; set; }
        public string SiteName { get; set; }
        public string SiteUrl { get; set; }
        public string InitialPassword { get; set; }
        public string Email { get; set; }
        public string EmailPassword { get; set; }
        public string EmailSMTP { get; set; }
        public int EmailPort { get; set; }
        public string PhoneNumber { get; set; }
        public string SupportEmail { get; set; }
        public string Address { get; set; }
        public int DefaultPageSize { get; set; }
        public string FileRootFolder { get; set; }
        public string BBBServer { get; set; }
        public string BBBAPIKey { get; set; }
        public bool IsPolicyEnabled { get; set; }
        public ConfigurationProtocolType ProtocolType { get; set; }
        public int DeepRoleCompetencyAnalysis { get; set; }
        public float MinimumGrade { get; set; }
        //public VOJobRole DefaultManagerJobRole { get; set; }
        public ConfigurationGradingScale GradingScale { get; set; }

        #endregion

        #region Constructors

        public VOConfiguration()
        {
        }

        #endregion

        #region Enums

        public enum ConfigurationProtocolType
        {
            HTTP = 0,
            HTTPS = 1
        }

        public enum ConfigurationGradingScale
        {
            GRADE_0_10 = 0,
            GRADE_0_100 = 1
        }

        #endregion
    }
}
