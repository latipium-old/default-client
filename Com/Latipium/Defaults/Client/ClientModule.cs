// ClientModule.cs
//
// Copyright (c) 2016 Zach Deibert.
// All Rights Reserved.
using System;
using System.Threading;
using log4net;
using Com.Latipium.Core;

namespace Com.Latipium.Defaults.Client {
	/// <summary>
	/// The default module implementation for the client.
	/// </summary>
	public class ClientModule : AbstractLatipiumModule {
		private static ILog Log = LogManager.GetLogger(typeof(ClientModule));

		/// <summary>
		/// Starts the client.
		/// </summary>
		[LatipiumMethod("Start")]
		public void Start() {
			LatipiumModule auth = ModuleFactory.FindModule("Com.Latipium.Modules.Authentication");
			LatipiumModule graphics = ModuleFactory.FindModule("Com.Latipium.Modules.Graphics");
			LatipiumModule network = ModuleFactory.FindModule("Com.Latipium.Modules.Network");
			LatipiumModule player = ModuleFactory.FindModule("Com.Latipium.Modules.Player");
			LatipiumModule world = ModuleFactory.FindModule("Com.Latipium.Modules.World");
			if ( graphics == null ) {
				Log.Error("Unable to find graphics module");
			} else {
				string name = null;
				LatipiumObject w = null;
				if ( world != null ) {
					if ( auth == null ) {
						name = "Player";
					} else {
						name = auth.InvokeFunction<string>("GetUsername");
					}
					w = world.InvokeFunction<LatipiumObject>("CreateWorld");
				}
				Thread networkThread = null;
				if ( network != null ) {
					Thread parent = Thread.CurrentThread;
					networkThread = new Thread(() => {
						network.InvokeProcedure("InitializeClient");
						if ( world != null ) {
							network.InvokeProcedure<LatipiumObject>("LoadWorld", w);
						}
						parent.Interrupt();
						try {
							network.InvokeProcedure("Loop");
						} catch ( ThreadInterruptedException ) {
						} finally {
							network.InvokeProcedure("Destroy");
						}
					});
					networkThread.Start();
					try {
						// Will be interrupted when the network loads
						Thread.Sleep(int.MaxValue);
					} catch ( ThreadInterruptedException ) {
					}
				}
				LatipiumObject p = null;
				if ( w != null ) {
					p = w.InvokeFunction<string, LatipiumObject>("GetPlayer", name);
					if ( player != null && p != null ) {
						player.InvokeProcedure<LatipiumObject>("HandleFor", p);
					}
				}
				graphics.InvokeProcedure("Initialize");
				if ( world != null ) {
					graphics.InvokeProcedure<LatipiumObject>("LoadWorld", w);
					if ( p != null ) {
						graphics.InvokeProcedure<LatipiumObject>("SetPlayer", p);
					}
				}
				try {
					graphics.InvokeProcedure("Loop");
				} finally {
					graphics.InvokeProcedure("Destroy");
					if ( networkThread != null ) {
						networkThread.Interrupt();
					}
				}
			}
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="Com.Latipium.Defaults.Client.ClientModule"/> class.
		/// </summary>
		public ClientModule() : base(new string[] { "Com.Latipium.Modules.Client" }) {
		}
	}
}

