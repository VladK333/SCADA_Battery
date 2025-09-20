using Common;
using System;
using System.Collections.Generic;
using System.Threading;

namespace ProcessingModule
{
    /// <summary>
    /// Class containing logic for automated work.
    /// </summary>
    public class AutomationManager : IAutomationManager, IDisposable
	{
		private Thread automationWorker;
        private AutoResetEvent automationTrigger;
        private IStorage storage;
		private IProcessingManager processingManager;
		private int delayBetweenCommands;
        private IConfiguration configuration;

        /// <summary>
        /// Initializes a new instance of the <see cref="AutomationManager"/> class.
        /// </summary>
        /// <param name="storage">The storage.</param>
        /// <param name="processingManager">The processing manager.</param>
        /// <param name="automationTrigger">The automation trigger.</param>
        /// <param name="configuration">The configuration.</param>
        public AutomationManager(IStorage storage, IProcessingManager processingManager, AutoResetEvent automationTrigger, IConfiguration configuration)
		{
			this.storage = storage;
			this.processingManager = processingManager;
            this.configuration = configuration;
            this.automationTrigger = automationTrigger;
        }

        /// <summary>
        /// Initializes and starts the threads.
        /// </summary>
		private void InitializeAndStartThreads()
		{
			InitializeAutomationWorkerThread();
			StartAutomationWorkerThread();
		}

        /// <summary>
        /// Initializes the automation worker thread.
        /// </summary>
		private void InitializeAutomationWorkerThread()
		{
			automationWorker = new Thread(AutomationWorker_DoWork);
			automationWorker.Name = "Aumation Thread";
		}

        /// <summary>
        /// Starts the automation worker thread.
        /// </summary>
		private void StartAutomationWorkerThread()
		{
			automationWorker.Start();
		}


		private void AutomationWorker_DoWork()
		{
            EGUConverter egu = new EGUConverter();
            PointIdentifier usb1 = new PointIdentifier(PointType.DIGITAL_OUTPUT, 1000);
            PointIdentifier usb2 = new PointIdentifier(PointType.DIGITAL_OUTPUT, 1001);
            PointIdentifier usb3 = new PointIdentifier(PointType.DIGITAL_OUTPUT, 1002);
            PointIdentifier socket = new PointIdentifier(PointType.DIGITAL_OUTPUT, 1003);
            PointIdentifier battery = new PointIdentifier(PointType.ANALOG_OUTPUT, 2000);
            PointIdentifier power1 = new PointIdentifier(PointType.DIGITAL_OUTPUT, 3000);
            PointIdentifier power2 = new PointIdentifier(PointType.DIGITAL_OUTPUT, 3001);

            List<PointIdentifier> list = new List<PointIdentifier>
                { usb1, usb2, usb3, socket, battery, power1, power2};

            int value = 0;

            while (!disposedValue)
            {
                List<IPoint> points = storage.GetPoints(list);

                value = (int)egu.ConvertToEGU(points[4].ConfigItem.ScaleFactor, points[4].ConfigItem.Deviation, points[4].RawValue);

                if (points[0].RawValue == 1 || points[1].RawValue == 1 || points[2].RawValue == 1)
                {
                    value -= 1;
                }
                if (points[3].RawValue == 1)
                {
                    value -= 3;
                }
                if (points[5].RawValue == 1)
                {
                    value += 3;
                }
                if (points[6].RawValue == 1)
                {
                    value += 4;
                }

                if (value <= points[4].ConfigItem.LowLimit)
                {
                    processingManager.ExecuteWriteCommand(points[4].ConfigItem, configuration.GetTransactionId(), configuration.UnitAddress, battery.Address, value);

                    processingManager.ExecuteWriteCommand(points[3].ConfigItem, configuration.GetTransactionId(), configuration.UnitAddress, socket.Address, 0);

                    processingManager.ExecuteWriteCommand(points[5].ConfigItem, configuration.GetTransactionId(), configuration.UnitAddress, power1.Address, 1);
                    processingManager.ExecuteWriteCommand(points[6].ConfigItem, configuration.GetTransactionId(), configuration.UnitAddress, power2.Address, 1);
                }
                else if (value >= points[4].ConfigItem.EGU_Max)
                {
                    value = 100;
                    processingManager.ExecuteWriteCommand(points[4].ConfigItem, configuration.GetTransactionId(), configuration.UnitAddress, battery.Address, value);

                    processingManager.ExecuteWriteCommand(points[5].ConfigItem, configuration.GetTransactionId(), configuration.UnitAddress, power1.Address, 0);
                    processingManager.ExecuteWriteCommand(points[6].ConfigItem, configuration.GetTransactionId(), configuration.UnitAddress, power2.Address, 0);
                }
                else
                {
                    processingManager.ExecuteWriteCommand(points[4].ConfigItem, configuration.GetTransactionId(), configuration.UnitAddress, battery.Address, value);
                }

               
                automationTrigger.WaitOne();
               
            }
        }

		#region IDisposable Support
		private bool disposedValue = false; // To detect redundant calls


        /// <summary>
        /// Disposes the object.
        /// </summary>
        /// <param name="disposing">Indication if managed objects should be disposed.</param>
		protected virtual void Dispose(bool disposing)
		{
			if (!disposedValue)
			{
				if (disposing)
				{
				}
				disposedValue = true;
			}
		}


		// This code added to correctly implement the disposable pattern.
		public void Dispose()
		{
			// Do not change this code. Put cleanup code in Dispose(bool disposing) above.
			Dispose(true);
			// GC.SuppressFinalize(this);
		}

        /// <inheritdoc />
        public void Start(int delayBetweenCommands)
		{
			this.delayBetweenCommands = delayBetweenCommands*1000;
            InitializeAndStartThreads();
		}

        /// <inheritdoc />
        public void Stop()
		{
			Dispose();
		}
		#endregion
	}
}
