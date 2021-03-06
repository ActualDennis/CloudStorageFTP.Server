﻿using DenCloud.Core.Authentication;
using DenCloud.Core.Connections;
using DenCloud.Core.Factories;
using DenCloud.Core.FileSystem;
using DenCloud.Core.Helpers;
using DenCloud.Core.Logging;
using DenInject.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DenCloud.Core.Di
{
    /// <summary>
    /// You must use this class to create a config and then create <see cref="DependencyProvider"/> with it.
    /// </summary>
    public class DiConfigBuilder
    {
        public DiConfigBuilder()
        {
            this.config = new DiConfiguration();
        }

        /// <summary>
        /// Config to use while DiContainer construction
        /// </summary>
        public DiConfiguration config { get; set; }

        public DiConfigFlags ConfigFlags { get; set; }

        /// <summary>
        /// Value indicates if this builder is ready to be passed to DiContainer.
        /// </summary>
        public bool Constructed
        {
            get =>
                ConfigFlags.HasFlag(DiConfigFlags.NecessaryClassesUsed)
                && ConfigFlags.HasFlag(DiConfigFlags.LoggerUsed)
                && ConfigFlags.HasFlag(DiConfigFlags.FilesystemUsed)
                && ConfigFlags.HasFlag(DiConfigFlags.AuthUsed);
        }

        /// <summary>
        /// You must call this method to obtain minimum ftp server functionality.
        /// </summary>
        public void UseNeccessaryClasses()
        {
            config.RegisterTransient<FtpServer, FtpServer>();
            config.RegisterTransient<ApplicationDbContext, ApplicationDbContext>();
            config.RegisterTransient<DataConnection, DataConnection>();
            config.RegisterTransient<FtpCommandFactory, FtpCommandFactory>();
            config.RegisterSingleton<DatabaseHelper, DatabaseHelper>();
            config.RegisterTransient<ControlConnection, ControlConnection>();
            ConfigFlags |= DiConfigFlags.NecessaryClassesUsed;
        }

        /// <summary>
        /// Use default logging <see cref="AutomaticFileLogger"/>
        /// </summary>
        public void UseLogger()
        {
            UseLogger(typeof(AutomaticFileLogger));
        }

        /// <summary>
        /// Use default auth <see cref="FtpDbAuthenticationProvider"/>
        /// </summary>
        public void UseAuthentication()
        {
            UseAuthentication(typeof(FtpDbAuthenticationProvider));
        }

        /// <summary>
        /// Use default filesystem <see cref="CloudStorageUnixFileSystemProvider"/>
        /// </summary>
        public void UseFileSystem()
        {
            UseFileSystem(typeof(CloudStorageUnixFileSystemProvider));
        }

        /// <summary>
        ///  You must call this method to obtain logger.
        /// </summary>
        /// <typeparam name="TLogger">Type of logger to use</typeparam>
        public void UseLogger<TLogger>() where TLogger : ILogger
        {
            UseLogger(typeof(TLogger));
        }

        /// <summary>
        ///  You must call this method to obtain authentication.
        /// </summary>
        /// <typeparam name="TAuthentication">Type of auth to use</typeparam>
        public void UseAuthentication<TAuthentication>() where TAuthentication : IAuthenticationProvider
        {
            UseAuthentication(typeof(TAuthentication));
        }

        /// <summary>
        ///  You must call this method to obtain filesystem.
        /// </summary>
        /// <typeparam name="TFileSystem">Type of filesystem to use</typeparam>
        public void UseFileSystem<TFileSystem>() 
            where TFileSystem : IFtpFileSystemProvider<FileSystemEntry>
        {
            UseFileSystem(typeof(TFileSystem));
        }

        private void UseLogger(Type loggerType)
        {     
            config.RegisterSingleton(typeof(ILogger), loggerType);
            ConfigFlags |= DiConfigFlags.LoggerUsed;
        }

        private void UseAuthentication(Type authType)
        {
            config.RegisterSingleton(typeof(IAuthenticationProvider), authType);
            ConfigFlags |= DiConfigFlags.AuthUsed;
        }

        private void UseFileSystem(Type filesystemType)
        {
            config.RegisterTransient(typeof(IFtpFileSystemProvider<FileSystemEntry>), filesystemType);
            ConfigFlags |= DiConfigFlags.FilesystemUsed;
        }
    }
}
