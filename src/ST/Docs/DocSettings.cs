﻿using System;
using System.IO;
using FubuCore;
using ST.Docs.Html;

namespace ST.Docs
{
    public enum UrlStyle
    {
        WebsiteExport,
        Live,
        FileExport
    }

    public class DocSettings
    {
        public DocSettings()
        {
            UrlStyle = UrlStyle.Live;
            Version = "Unknown";
        }

        public UrlStyle UrlStyle { get; set; }
        public string Root { get; set; }
        public string WebsocketAddress { get; set; }
        public string Version { get; set; }
        public string[] SampleDirectories { get; set; }

        public Type UrlResolverType()
        {
            switch (UrlStyle)
            {
                case UrlStyle.Live:
                    return typeof (LiveUrlResolver);

                case UrlStyle.WebsiteExport:
                    return typeof (LiveUrlResolver);

                case UrlStyle.FileExport:
                    return typeof (FileExportUrlResolver);
            }

            throw new ArgumentOutOfRangeException("Shouldn't be possible");
        }

        public static DocSettings ForTesting()
        {
            var directory = Path.GetTempPath().AppendPath(Guid.NewGuid().ToString());
            new FileSystem().CreateDirectory(directory);
            return new DocSettings
            {
                Root = directory
            };
        }
    }
}