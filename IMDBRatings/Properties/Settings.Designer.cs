﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace IMDBRatings.Properties {
    
    
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.VisualStudio.Editors.SettingsDesigner.SettingsSingleFileGenerator", "16.7.0.0")]
    internal sealed partial class Settings : global::System.Configuration.ApplicationSettingsBase {
        
        private static Settings defaultInstance = ((Settings)(global::System.Configuration.ApplicationSettingsBase.Synchronized(new Settings())));
        
        public static Settings Default {
            get {
                return defaultInstance;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("whattowatchon@theslaveys.com")]
        public string FtpUsername {
            get {
                return ((string)(this["FtpUsername"]));
            }
            set {
                this["FtpUsername"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("ftp://ftp.theslaveys.com")]
        public string FtpAddress {
            get {
                return ((string)(this["FtpAddress"]));
            }
            set {
                this["FtpAddress"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("pHa2srppV58eN7$q")]
        public string FtpPassword {
            get {
                return ((string)(this["FtpPassword"]));
            }
            set {
                this["FtpPassword"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("E:\\Raw\\tvRatingsJsonFiles")]
        public string FilePath {
            get {
                return ((string)(this["FilePath"]));
            }
            set {
                this["FilePath"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("title.basics.tsv.gz,title.episode.tsv.gz,title.ratings.tsv.gz")]
        public string IMDBFileNames {
            get {
                return ((string)(this["IMDBFileNames"]));
            }
            set {
                this["IMDBFileNames"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("https://datasets.imdbws.com/")]
        public string IMDBUrlPath {
            get {
                return ((string)(this["IMDBUrlPath"]));
            }
            set {
                this["IMDBUrlPath"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("https://whattowatchon.tv/")]
        public string TargetWebsite {
            get {
                return ((string)(this["TargetWebsite"]));
            }
            set {
                this["TargetWebsite"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("daily")]
        public string SitemapChangeFrequency {
            get {
                return ((string)(this["SitemapChangeFrequency"]));
            }
            set {
                this["SitemapChangeFrequency"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("False")]
        public bool IsDebug {
            get {
                return ((bool)(this["IsDebug"]));
            }
            set {
                this["IsDebug"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("100")]
        public short TopShowsMinVotesRequired {
            get {
                return ((short)(this["TopShowsMinVotesRequired"]));
            }
            set {
                this["TopShowsMinVotesRequired"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("4")]
        public short TopShowsMinEpisodeCount {
            get {
                return ((short)(this["TopShowsMinEpisodeCount"]));
            }
            set {
                this["TopShowsMinEpisodeCount"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("10")]
        public short TopShowsMinCount {
            get {
                return ((short)(this["TopShowsMinCount"]));
            }
            set {
                this["TopShowsMinCount"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("1600")]
        public short ViewportWidthForScreenshot {
            get {
                return ((short)(this["ViewportWidthForScreenshot"]));
            }
            set {
                this["ViewportWidthForScreenshot"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("250")]
        public short ScreenshotsToGenerate {
            get {
                return ((short)(this["ScreenshotsToGenerate"]));
            }
            set {
                this["ScreenshotsToGenerate"] = value;
            }
        }
    }
}
