using System;

using NuciLog.Core;

using WirelessRouterRebooter.Logging;
using WirelessRouterRebooter.Service.Models;
using WirelessRouterRebooter.Service.Processors;

namespace WirelessRouterRebooter.Service
{
    public class BotService(
        IRouterProcessor routerProcessor,
        ILogger logger) : IBotService
    {
        public void Run(UserCredentials userCredentials)
        {
            LogIn(userCredentials);
            Reboot();
        }

        void LogIn(UserCredentials userCredentials)
        {
            logger.Info(
                MyOperation.LogIn,
                OperationStatus.Started,
                new LogInfo(MyLogInfoKey.IpAddress, routerProcessor.IpAddress),
                new LogInfo(MyLogInfoKey.BrandName, routerProcessor.BrandName),
                new LogInfo(MyLogInfoKey.ModelName, routerProcessor.ModelName),
                new LogInfo(MyLogInfoKey.Username, userCredentials.Username));

            try
            {
                routerProcessor.LogIn(userCredentials);

                logger.Debug(
                    MyOperation.LogIn,
                    OperationStatus.Success,
                    new LogInfo(MyLogInfoKey.IpAddress, routerProcessor.IpAddress),
                    new LogInfo(MyLogInfoKey.BrandName, routerProcessor.BrandName),
                    new LogInfo(MyLogInfoKey.ModelName, routerProcessor.ModelName),
                    new LogInfo(MyLogInfoKey.Username, userCredentials.Username));
            }
            catch (Exception ex)
            {
                logger.Error(
                    MyOperation.LogIn,
                    OperationStatus.Failure,
                    ex,
                    new LogInfo(MyLogInfoKey.IpAddress, routerProcessor.IpAddress),
                    new LogInfo(MyLogInfoKey.BrandName, routerProcessor.BrandName),
                    new LogInfo(MyLogInfoKey.ModelName, routerProcessor.ModelName),
                    new LogInfo(MyLogInfoKey.Username, userCredentials.Username));

                throw;
            }
        }

        void Reboot()
        {
            logger.Info(
                MyOperation.Reboot,
                OperationStatus.Started,
                new LogInfo(MyLogInfoKey.IpAddress, routerProcessor.IpAddress),
                new LogInfo(MyLogInfoKey.BrandName, routerProcessor.BrandName),
                new LogInfo(MyLogInfoKey.ModelName, routerProcessor.ModelName));

            try
            {
                routerProcessor.Reboot();

                logger.Debug(
                    MyOperation.Reboot,
                    OperationStatus.Success,
                    new LogInfo(MyLogInfoKey.IpAddress, routerProcessor.IpAddress),
                    new LogInfo(MyLogInfoKey.BrandName, routerProcessor.BrandName),
                    new LogInfo(MyLogInfoKey.ModelName, routerProcessor.ModelName));
            }
            catch (Exception ex)
            {
                logger.Error(
                    MyOperation.Reboot,
                    OperationStatus.Failure,
                    ex,
                    new LogInfo(MyLogInfoKey.IpAddress, routerProcessor.IpAddress),
                    new LogInfo(MyLogInfoKey.BrandName, routerProcessor.BrandName),
                    new LogInfo(MyLogInfoKey.ModelName, routerProcessor.ModelName));

                throw;
            }
        }
    }
}
