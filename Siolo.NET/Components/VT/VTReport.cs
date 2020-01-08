namespace Siolo.NET.Components.VT
{
    public class VTReport
    {
        public Data data { get; set; }
        public object error { get; set; }
    }

    public class Data
    {
        public Attributes attributes { get; set; }
        public string id { get; set; }
        public Links links { get; set; }
        public string type { get; set; }
    }

    public class Attributes
    {
        public string authentihash { get; set; }
        public int creation_date { get; set; }
        public Exiftool exiftool { get; set; }
        public int first_submission_date { get; set; }
        public int last_analysis_date { get; set; }
        public Last_Analysis_Results last_analysis_results { get; set; }
        public Last_Analysis_Stats last_analysis_stats { get; set; }
        public int last_modification_date { get; set; }
        public int last_submission_date { get; set; }
        public string magic { get; set; }
        public string md5 { get; set; }
        public string meaningful_name { get; set; }
        public string[] names { get; set; }
        public Packers packers { get; set; }
        public Pe_Info pe_info { get; set; }
        public int reputation { get; set; }
        public string sha1 { get; set; }
        public string sha256 { get; set; }
        public Signature_Info signature_info { get; set; }
        public int size { get; set; }
        public string ssdeep { get; set; }
        public string[] tags { get; set; }
        public int times_submitted { get; set; }
        public Total_Votes total_votes { get; set; }
        public Trid[] trid { get; set; }
        public string type_description { get; set; }
        public string type_tag { get; set; }
        public int unique_sources { get; set; }
        public string vhash { get; set; }
    }

    public class Exiftool
    {
        public string CharacterSet { get; set; }
        public int CodeSize { get; set; }
        public string CompanyName { get; set; }
        public string EntryPoint { get; set; }
        public string FileDescription { get; set; }
        public string FileFlagsMask { get; set; }
        public string FileOS { get; set; }
        public int FileSubtype { get; set; }
        public string FileType { get; set; }
        public string FileTypeExtension { get; set; }
        public string FileVersion { get; set; }
        public string FileVersionNumber { get; set; }
        public string ImageFileCharacteristics { get; set; }
        public float ImageVersion { get; set; }
        public int InitializedDataSize { get; set; }
        public string InternalName { get; set; }
        public string LanguageCode { get; set; }
        public string LegalCopyright { get; set; }
        public float LinkerVersion { get; set; }
        public string MIMEType { get; set; }
        public string MachineType { get; set; }
        public float OSVersion { get; set; }
        public string ObjectFileType { get; set; }
        public string OriginalFileName { get; set; }
        public string PEType { get; set; }
        public string ProductName { get; set; }
        public string ProductVersion { get; set; }
        public string ProductVersionNumber { get; set; }
        public string Subsystem { get; set; }
        public float SubsystemVersion { get; set; }
        public string TimeStamp { get; set; }
        public int UninitializedDataSize { get; set; }
    }

    public class Last_Analysis_Results
    {
        public Alyac ALYac { get; set; }
        public APEX APEX { get; set; }
        public AVG AVG { get; set; }
        public Acronis Acronis { get; set; }
        public AdAware AdAware { get; set; }
        public Aegislab AegisLab { get; set; }
        public AhnlabV3 AhnLabV3 { get; set; }
        public Alibaba Alibaba { get; set; }
        public AntiyAVL AntiyAVL { get; set; }
        public Arcabit Arcabit { get; set; }
        public Avast Avast { get; set; }
        public AvastMobile AvastMobile { get; set; }
        public Avira Avira { get; set; }
        public Baidu Baidu { get; set; }
        public Bitdefender BitDefender { get; set; }
        public Bitdefendertheta BitDefenderTheta { get; set; }
        public Bkav Bkav { get; set; }
        public CATQuickheal CATQuickHeal { get; set; }
        public CMC CMC { get; set; }
        public Clamav ClamAV { get; set; }
        public Comodo Comodo { get; set; }
        public Crowdstrike CrowdStrike { get; set; }
        public Cybereason Cybereason { get; set; }
        public Cylance Cylance { get; set; }
        public Cyren Cyren { get; set; }
        public Drweb DrWeb { get; set; }
        public ESETNOD32 ESETNOD32 { get; set; }
        public Emsisoft Emsisoft { get; set; }
        public Endgame Endgame { get; set; }
        public FProt FProt { get; set; }
        public FSecure FSecure { get; set; }
        public Fireeye FireEye { get; set; }
        public Fortinet Fortinet { get; set; }
        public Gdata GData { get; set; }
        public Ikarus Ikarus { get; set; }
        public Invincea Invincea { get; set; }
        public Jiangmin Jiangmin { get; set; }
        public K7antivirus K7AntiVirus { get; set; }
        public K7GW K7GW { get; set; }
        public Kaspersky Kaspersky { get; set; }
        public Kingsoft Kingsoft { get; set; }
        public MAX MAX { get; set; }
        public Malwarebytes Malwarebytes { get; set; }
        public Maxsecure MaxSecure { get; set; }
        public Mcafee McAfee { get; set; }
        public McafeeGWEdition McAfeeGWEdition { get; set; }
        public MicroworldEscan MicroWorldeScan { get; set; }
        public Microsoft Microsoft { get; set; }
        public NANOAntivirus NANOAntivirus { get; set; }
        public Paloalto Paloalto { get; set; }
        public Panda Panda { get; set; }
        public Qihoo360 Qihoo360 { get; set; }
        public Rising Rising { get; set; }
        public Superantispyware SUPERAntiSpyware { get; set; }
        public Sangfor Sangfor { get; set; }
        public Sentinelone SentinelOne { get; set; }
        public Sophos Sophos { get; set; }
        public Symantec Symantec { get; set; }
        public Symantecmobileinsight SymantecMobileInsight { get; set; }
        public TACHYON TACHYON { get; set; }
        public Tencent Tencent { get; set; }
        public Totaldefense TotalDefense { get; set; }
        public Trapmine Trapmine { get; set; }
        public Trendmicro TrendMicro { get; set; }
        public TrendmicroHousecall TrendMicroHouseCall { get; set; }
        public Trustlook Trustlook { get; set; }
        public VBA32 VBA32 { get; set; }
        public VIPRE VIPRE { get; set; }
        public Virobot ViRobot { get; set; }
        public Webroot Webroot { get; set; }
        public Yandex Yandex { get; set; }
        public Zillya Zillya { get; set; }
        public Zonealarm ZoneAlarm { get; set; }
        public Zoner Zoner { get; set; }
        public Egambit eGambit { get; set; }
    }

    public class Alyac
    {
        public string category { get; set; }
        public string engine_name { get; set; }
        public string engine_update { get; set; }
        public string engine_version { get; set; }
        public string method { get; set; }
        public string result { get; set; }
    }

    public class APEX
    {
        public string category { get; set; }
        public string engine_name { get; set; }
        public string engine_update { get; set; }
        public string engine_version { get; set; }
        public string method { get; set; }
        public object result { get; set; }
    }

    public class AVG
    {
        public string category { get; set; }
        public string engine_name { get; set; }
        public string engine_update { get; set; }
        public string engine_version { get; set; }
        public string method { get; set; }
        public string result { get; set; }
    }

    public class Acronis
    {
        public string category { get; set; }
        public string engine_name { get; set; }
        public string engine_update { get; set; }
        public string engine_version { get; set; }
        public string method { get; set; }
        public object result { get; set; }
    }

    public class AdAware
    {
        public string category { get; set; }
        public string engine_name { get; set; }
        public string engine_update { get; set; }
        public string engine_version { get; set; }
        public string method { get; set; }
        public string result { get; set; }
    }

    public class Aegislab
    {
        public string category { get; set; }
        public string engine_name { get; set; }
        public string engine_update { get; set; }
        public string engine_version { get; set; }
        public string method { get; set; }
        public string result { get; set; }
    }

    public class AhnlabV3
    {
        public string category { get; set; }
        public string engine_name { get; set; }
        public string engine_update { get; set; }
        public string engine_version { get; set; }
        public string method { get; set; }
        public string result { get; set; }
    }

    public class Alibaba
    {
        public string category { get; set; }
        public string engine_name { get; set; }
        public string engine_update { get; set; }
        public string engine_version { get; set; }
        public string method { get; set; }
        public string result { get; set; }
    }

    public class AntiyAVL
    {
        public string category { get; set; }
        public string engine_name { get; set; }
        public string engine_update { get; set; }
        public string engine_version { get; set; }
        public string method { get; set; }
        public string result { get; set; }
    }

    public class Arcabit
    {
        public string category { get; set; }
        public string engine_name { get; set; }
        public string engine_update { get; set; }
        public string engine_version { get; set; }
        public string method { get; set; }
        public string result { get; set; }
    }

    public class Avast
    {
        public string category { get; set; }
        public string engine_name { get; set; }
        public string engine_update { get; set; }
        public string engine_version { get; set; }
        public string method { get; set; }
        public string result { get; set; }
    }

    public class AvastMobile
    {
        public string category { get; set; }
        public string engine_name { get; set; }
        public string engine_update { get; set; }
        public string engine_version { get; set; }
        public string method { get; set; }
        public object result { get; set; }
    }

    public class Avira
    {
        public string category { get; set; }
        public string engine_name { get; set; }
        public string engine_update { get; set; }
        public string engine_version { get; set; }
        public string method { get; set; }
        public string result { get; set; }
    }

    public class Baidu
    {
        public string category { get; set; }
        public string engine_name { get; set; }
        public string engine_update { get; set; }
        public string engine_version { get; set; }
        public string method { get; set; }
        public object result { get; set; }
    }

    public class Bitdefender
    {
        public string category { get; set; }
        public string engine_name { get; set; }
        public string engine_update { get; set; }
        public string engine_version { get; set; }
        public string method { get; set; }
        public string result { get; set; }
    }

    public class Bitdefendertheta
    {
        public string category { get; set; }
        public string engine_name { get; set; }
        public string engine_update { get; set; }
        public string engine_version { get; set; }
        public string method { get; set; }
        public string result { get; set; }
    }

    public class Bkav
    {
        public string category { get; set; }
        public string engine_name { get; set; }
        public string engine_update { get; set; }
        public string engine_version { get; set; }
        public string method { get; set; }
        public object result { get; set; }
    }

    public class CATQuickheal
    {
        public string category { get; set; }
        public string engine_name { get; set; }
        public string engine_update { get; set; }
        public string engine_version { get; set; }
        public string method { get; set; }
        public string result { get; set; }
    }

    public class CMC
    {
        public string category { get; set; }
        public string engine_name { get; set; }
        public string engine_update { get; set; }
        public string engine_version { get; set; }
        public string method { get; set; }
        public object result { get; set; }
    }

    public class Clamav
    {
        public string category { get; set; }
        public string engine_name { get; set; }
        public string engine_update { get; set; }
        public string engine_version { get; set; }
        public string method { get; set; }
        public string result { get; set; }
    }

    public class Comodo
    {
        public string category { get; set; }
        public string engine_name { get; set; }
        public string engine_update { get; set; }
        public string engine_version { get; set; }
        public string method { get; set; }
        public string result { get; set; }
    }

    public class Crowdstrike
    {
        public string category { get; set; }
        public string engine_name { get; set; }
        public string engine_update { get; set; }
        public string engine_version { get; set; }
        public string method { get; set; }
        public object result { get; set; }
    }

    public class Cybereason
    {
        public string category { get; set; }
        public string engine_name { get; set; }
        public string engine_update { get; set; }
        public string engine_version { get; set; }
        public string method { get; set; }
        public object result { get; set; }
    }

    public class Cylance
    {
        public string category { get; set; }
        public string engine_name { get; set; }
        public string engine_update { get; set; }
        public string engine_version { get; set; }
        public string method { get; set; }
        public string result { get; set; }
    }

    public class Cyren
    {
        public string category { get; set; }
        public string engine_name { get; set; }
        public string engine_update { get; set; }
        public string engine_version { get; set; }
        public string method { get; set; }
        public object result { get; set; }
    }

    public class Drweb
    {
        public string category { get; set; }
        public string engine_name { get; set; }
        public string engine_update { get; set; }
        public string engine_version { get; set; }
        public string method { get; set; }
        public string result { get; set; }
    }

    public class ESETNOD32
    {
        public string category { get; set; }
        public string engine_name { get; set; }
        public string engine_update { get; set; }
        public string engine_version { get; set; }
        public string method { get; set; }
        public string result { get; set; }
    }

    public class Emsisoft
    {
        public string category { get; set; }
        public string engine_name { get; set; }
        public string engine_update { get; set; }
        public string engine_version { get; set; }
        public string method { get; set; }
        public string result { get; set; }
    }

    public class Endgame
    {
        public string category { get; set; }
        public string engine_name { get; set; }
        public string engine_update { get; set; }
        public string engine_version { get; set; }
        public string method { get; set; }
        public string result { get; set; }
    }

    public class FProt
    {
        public string category { get; set; }
        public string engine_name { get; set; }
        public string engine_update { get; set; }
        public string engine_version { get; set; }
        public string method { get; set; }
        public object result { get; set; }
    }

    public class FSecure
    {
        public string category { get; set; }
        public string engine_name { get; set; }
        public string engine_update { get; set; }
        public string engine_version { get; set; }
        public string method { get; set; }
        public string result { get; set; }
    }

    public class Fireeye
    {
        public string category { get; set; }
        public string engine_name { get; set; }
        public string engine_update { get; set; }
        public string engine_version { get; set; }
        public string method { get; set; }
        public string result { get; set; }
    }

    public class Fortinet
    {
        public string category { get; set; }
        public string engine_name { get; set; }
        public string engine_update { get; set; }
        public string engine_version { get; set; }
        public string method { get; set; }
        public string result { get; set; }
    }

    public class Gdata
    {
        public string category { get; set; }
        public string engine_name { get; set; }
        public string engine_update { get; set; }
        public string engine_version { get; set; }
        public string method { get; set; }
        public string result { get; set; }
    }

    public class Ikarus
    {
        public string category { get; set; }
        public string engine_name { get; set; }
        public string engine_update { get; set; }
        public string engine_version { get; set; }
        public string method { get; set; }
        public string result { get; set; }
    }

    public class Invincea
    {
        public string category { get; set; }
        public string engine_name { get; set; }
        public string engine_update { get; set; }
        public string engine_version { get; set; }
        public string method { get; set; }
        public object result { get; set; }
    }

    public class Jiangmin
    {
        public string category { get; set; }
        public string engine_name { get; set; }
        public string engine_update { get; set; }
        public string engine_version { get; set; }
        public string method { get; set; }
        public string result { get; set; }
    }

    public class K7antivirus
    {
        public string category { get; set; }
        public string engine_name { get; set; }
        public string engine_update { get; set; }
        public string engine_version { get; set; }
        public string method { get; set; }
        public string result { get; set; }
    }

    public class K7GW
    {
        public string category { get; set; }
        public string engine_name { get; set; }
        public string engine_update { get; set; }
        public string engine_version { get; set; }
        public string method { get; set; }
        public string result { get; set; }
    }

    public class Kaspersky
    {
        public string category { get; set; }
        public string engine_name { get; set; }
        public string engine_update { get; set; }
        public string engine_version { get; set; }
        public string method { get; set; }
        public string result { get; set; }
    }

    public class Kingsoft
    {
        public string category { get; set; }
        public string engine_name { get; set; }
        public string engine_update { get; set; }
        public string engine_version { get; set; }
        public string method { get; set; }
        public object result { get; set; }
    }

    public class MAX
    {
        public string category { get; set; }
        public string engine_name { get; set; }
        public string engine_update { get; set; }
        public string engine_version { get; set; }
        public string method { get; set; }
        public string result { get; set; }
    }

    public class Malwarebytes
    {
        public string category { get; set; }
        public string engine_name { get; set; }
        public string engine_update { get; set; }
        public string engine_version { get; set; }
        public string method { get; set; }
        public object result { get; set; }
    }

    public class Maxsecure
    {
        public string category { get; set; }
        public string engine_name { get; set; }
        public string engine_update { get; set; }
        public string engine_version { get; set; }
        public string method { get; set; }
        public object result { get; set; }
    }

    public class Mcafee
    {
        public string category { get; set; }
        public string engine_name { get; set; }
        public string engine_update { get; set; }
        public string engine_version { get; set; }
        public string method { get; set; }
        public string result { get; set; }
    }

    public class McafeeGWEdition
    {
        public string category { get; set; }
        public string engine_name { get; set; }
        public string engine_update { get; set; }
        public string engine_version { get; set; }
        public string method { get; set; }
        public string result { get; set; }
    }

    public class MicroworldEscan
    {
        public string category { get; set; }
        public string engine_name { get; set; }
        public string engine_update { get; set; }
        public string engine_version { get; set; }
        public string method { get; set; }
        public string result { get; set; }
    }

    public class Microsoft
    {
        public string category { get; set; }
        public string engine_name { get; set; }
        public string engine_update { get; set; }
        public string engine_version { get; set; }
        public string method { get; set; }
        public string result { get; set; }
    }

    public class NANOAntivirus
    {
        public string category { get; set; }
        public string engine_name { get; set; }
        public string engine_update { get; set; }
        public string engine_version { get; set; }
        public string method { get; set; }
        public string result { get; set; }
    }

    public class Paloalto
    {
        public string category { get; set; }
        public string engine_name { get; set; }
        public string engine_update { get; set; }
        public string engine_version { get; set; }
        public string method { get; set; }
        public string result { get; set; }
    }

    public class Panda
    {
        public string category { get; set; }
        public string engine_name { get; set; }
        public string engine_update { get; set; }
        public string engine_version { get; set; }
        public string method { get; set; }
        public string result { get; set; }
    }

    public class Qihoo360
    {
        public string category { get; set; }
        public string engine_name { get; set; }
        public string engine_update { get; set; }
        public string engine_version { get; set; }
        public string method { get; set; }
        public string result { get; set; }
    }

    public class Rising
    {
        public string category { get; set; }
        public string engine_name { get; set; }
        public string engine_update { get; set; }
        public string engine_version { get; set; }
        public string method { get; set; }
        public string result { get; set; }
    }

    public class Superantispyware
    {
        public string category { get; set; }
        public string engine_name { get; set; }
        public string engine_update { get; set; }
        public string engine_version { get; set; }
        public string method { get; set; }
        public object result { get; set; }
    }

    public class Sangfor
    {
        public string category { get; set; }
        public string engine_name { get; set; }
        public string engine_update { get; set; }
        public string engine_version { get; set; }
        public string method { get; set; }
        public object result { get; set; }
    }

    public class Sentinelone
    {
        public string category { get; set; }
        public string engine_name { get; set; }
        public string engine_update { get; set; }
        public string engine_version { get; set; }
        public string method { get; set; }
        public object result { get; set; }
    }

    public class Sophos
    {
        public string category { get; set; }
        public string engine_name { get; set; }
        public string engine_update { get; set; }
        public string engine_version { get; set; }
        public string method { get; set; }
        public object result { get; set; }
    }

    public class Symantec
    {
        public string category { get; set; }
        public string engine_name { get; set; }
        public string engine_update { get; set; }
        public string engine_version { get; set; }
        public string method { get; set; }
        public string result { get; set; }
    }

    public class Symantecmobileinsight
    {
        public string category { get; set; }
        public string engine_name { get; set; }
        public string engine_update { get; set; }
        public string engine_version { get; set; }
        public string method { get; set; }
        public object result { get; set; }
    }

    public class TACHYON
    {
        public string category { get; set; }
        public string engine_name { get; set; }
        public string engine_update { get; set; }
        public string engine_version { get; set; }
        public string method { get; set; }
        public object result { get; set; }
    }

    public class Tencent
    {
        public string category { get; set; }
        public string engine_name { get; set; }
        public string engine_update { get; set; }
        public string engine_version { get; set; }
        public string method { get; set; }
        public object result { get; set; }
    }

    public class Totaldefense
    {
        public string category { get; set; }
        public string engine_name { get; set; }
        public string engine_update { get; set; }
        public string engine_version { get; set; }
        public string method { get; set; }
        public object result { get; set; }
    }

    public class Trapmine
    {
        public string category { get; set; }
        public string engine_name { get; set; }
        public string engine_update { get; set; }
        public string engine_version { get; set; }
        public string method { get; set; }
        public object result { get; set; }
    }

    public class Trendmicro
    {
        public string category { get; set; }
        public string engine_name { get; set; }
        public string engine_update { get; set; }
        public string engine_version { get; set; }
        public string method { get; set; }
        public string result { get; set; }
    }

    public class TrendmicroHousecall
    {
        public string category { get; set; }
        public string engine_name { get; set; }
        public string engine_update { get; set; }
        public string engine_version { get; set; }
        public string method { get; set; }
        public string result { get; set; }
    }

    public class Trustlook
    {
        public string category { get; set; }
        public string engine_name { get; set; }
        public string engine_update { get; set; }
        public string engine_version { get; set; }
        public string method { get; set; }
        public object result { get; set; }
    }

    public class VBA32
    {
        public string category { get; set; }
        public string engine_name { get; set; }
        public string engine_update { get; set; }
        public string engine_version { get; set; }
        public string method { get; set; }
        public string result { get; set; }
    }

    public class VIPRE
    {
        public string category { get; set; }
        public string engine_name { get; set; }
        public string engine_update { get; set; }
        public string engine_version { get; set; }
        public string method { get; set; }
        public string result { get; set; }
    }

    public class Virobot
    {
        public string category { get; set; }
        public string engine_name { get; set; }
        public string engine_update { get; set; }
        public string engine_version { get; set; }
        public string method { get; set; }
        public string result { get; set; }
    }

    public class Webroot
    {
        public string category { get; set; }
        public string engine_name { get; set; }
        public string engine_update { get; set; }
        public string engine_version { get; set; }
        public string method { get; set; }
        public string result { get; set; }
    }

    public class Yandex
    {
        public string category { get; set; }
        public string engine_name { get; set; }
        public string engine_update { get; set; }
        public string engine_version { get; set; }
        public string method { get; set; }
        public string result { get; set; }
    }

    public class Zillya
    {
        public string category { get; set; }
        public string engine_name { get; set; }
        public string engine_update { get; set; }
        public string engine_version { get; set; }
        public string method { get; set; }
        public string result { get; set; }
    }

    public class Zonealarm
    {
        public string category { get; set; }
        public string engine_name { get; set; }
        public string engine_update { get; set; }
        public string engine_version { get; set; }
        public string method { get; set; }
        public string result { get; set; }
    }

    public class Zoner
    {
        public string category { get; set; }
        public string engine_name { get; set; }
        public string engine_update { get; set; }
        public string engine_version { get; set; }
        public string method { get; set; }
        public object result { get; set; }
    }

    public class Egambit
    {
        public string category { get; set; }
        public string engine_name { get; set; }
        public string engine_update { get; set; }
        public object engine_version { get; set; }
        public string method { get; set; }
        public object result { get; set; }
    }

    public class Last_Analysis_Stats
    {
        public int confirmedtimeout { get; set; }
        public int failure { get; set; }
        public int harmless { get; set; }
        public int malicious { get; set; }
        public int suspicious { get; set; }
        public int timeout { get; set; }
        public int typeunsupported { get; set; }
        public int undetected { get; set; }
    }

    public class Packers
    {
        public string PEiD { get; set; }
    }

    public class Pe_Info
    {
        public int entry_point { get; set; }
        public string[] exports { get; set; }
        public string imphash { get; set; }
        public Imports imports { get; set; }
        public int machine_type { get; set; }
        public Resource_Details[] resource_details { get; set; }
        public Resource_Langs resource_langs { get; set; }
        public Resource_Types resource_types { get; set; }
        public Section[] sections { get; set; }
        public int timestamp { get; set; }
    }

    public class Imports
    {
        public string[] ADVAPI32dll { get; set; }
        public string[] KERNEL32dll { get; set; }
        public string[] USER32dll { get; set; }
        public string[] msvcrtdll { get; set; }
    }

    public class Resource_Langs
    {
        public int ENGLISHUS { get; set; }
    }

    public class Resource_Types
    {
        public int RT_VERSION { get; set; }
    }

    public class Resource_Details
    {
        public float chi2 { get; set; }
        public float entropy { get; set; }
        public string filetype { get; set; }
        public string lang { get; set; }
        public string sha256 { get; set; }
        public string type { get; set; }
    }

    public class Section
    {
        public float entropy { get; set; }
        public string md5 { get; set; }
        public string name { get; set; }
        public int raw_size { get; set; }
        public int virtual_address { get; set; }
        public int virtual_size { get; set; }
    }

    public class Signature_Info
    {
        public string copyright { get; set; }
        public string description { get; set; }
        public string fileversion { get; set; }
        public string internalname { get; set; }
        public string originalname { get; set; }
        public string product { get; set; }
    }

    public class Total_Votes
    {
        public int harmless { get; set; }
        public int malicious { get; set; }
    }

    public class Trid
    {
        public string file_type { get; set; }
        public float probability { get; set; }
    }

    public class Links
    {
        public string self { get; set; }
    }
}
