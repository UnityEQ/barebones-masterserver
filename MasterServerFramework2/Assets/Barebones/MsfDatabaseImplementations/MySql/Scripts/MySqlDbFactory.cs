#if (!UNITY_WEBGL && !UNITY_IOS) || UNITY_EDITOR
using System;
using System.Collections.Generic;
using MySql.Data.MySqlClient;
using UnityEngine;
#endif

namespace Barebones.MasterServer
{
    public class MySqlDbFactory : MonoBehaviour
    {
        public ServerBehaviour Server;

        public string DefaultConnectionString = "Server=127.0.0.1;Uid=root;Pwd=root;Database=msf2;";

        protected virtual void Awake()
        {
            Server = Server ?? GetComponentInParent<ServerBehaviour>();

            if (Server == null)
            {
                Logs.Error("Database Factory server is not set. Make sure db factory " +
                           "is a child of ServerBehaviour, or that the Server property is set");
                return;
            }

            Server.Started += OnServerStarted;

            // If server is already running
            if (Server.IsRunning)
                OnServerStarted();
        }

        protected virtual void OnServerStarted()
        {
#if (!UNITY_WEBGL && !UNITY_IOS) || UNITY_EDITOR
            try
            {
                var connectionString = Msf.Args.IsProvided(Msf.Args.Names.DbConnectionString)
                    ? Msf.Args.DbConnectionString
                    : DefaultConnectionString;

                Msf.Server.DbAccessors.SetAccessor<IAuthDatabase>(new AuthDbMysql(connectionString));
                Msf.Server.DbAccessors.SetAccessor<IProfilesDatabase>(new ProfilesDbMysql(connectionString));
            }
            catch
            {
                Logs.Error("Failed to connect to database");
                throw;
            }
#endif
        }

        protected virtual void OnDestroy()
        {
            if (Server != null)
                Server.Started -= OnServerStarted;
        }
    }
}

