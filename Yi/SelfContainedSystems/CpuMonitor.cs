using System;
using System.Diagnostics;
using System.Runtime.InteropServices.ComTypes;
using System.Threading;
using Yi.Database.Converters;

#pragma warning disable 618

namespace Yi.SelfContainedSystems
{
    public class CpuMonitor
    {
        private static CpuMonitor _instance;
        public static CpuMonitor Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new CpuMonitor();
                    _instance.GetUsage();
                }
                return _instance;
            }
        }

        private FILETIME _prevSysKernel;
        private FILETIME _prevSysUser;

        private TimeSpan _prevProcTotal;

        private float _cpuUsage;
        private DateTime _lastRun;
        private long _runCount;

        public CpuMonitor()
        {
            _cpuUsage = -1;
            _lastRun = DateTime.MinValue;
            _prevSysUser.dwHighDateTime = _prevSysUser.dwLowDateTime = 0;
            _prevSysKernel.dwHighDateTime = _prevSysKernel.dwLowDateTime = 0;
            _prevProcTotal = TimeSpan.MinValue;
            _runCount = 0;
        }

        public float GetUsage()
        {
            var cpuCopy = _cpuUsage;
            if (Interlocked.Increment(ref _runCount) == 1)
            {
                if (!EnoughTimePassed)
                {
                    Interlocked.Decrement(ref _runCount);
                    return cpuCopy;
                }

                FILETIME sysIdle, sysKernel, sysUser;

                var process = Process.GetCurrentProcess();
                var procTime = process.TotalProcessorTime;

                if (!NativeMethods.GetSystemTimes(out sysIdle, out sysKernel, out sysUser))
                {
                    Interlocked.Decrement(ref _runCount);
                    return cpuCopy;
                }

                if (!IsFirstRun)
                {
                    var sysKernelDiff = SubtractTimes(sysKernel, _prevSysKernel);
                    var sysUserDiff = SubtractTimes(sysUser, _prevSysUser);

                    var sysTotal = sysKernelDiff + sysUserDiff;

                    var procTotal = procTime.Ticks - _prevProcTotal.Ticks;

                    if (sysTotal > 0)
                        _cpuUsage = 100.0f * procTotal / sysTotal*Environment.ProcessorCount;
                }

                _prevProcTotal = procTime;
                _prevSysKernel = sysKernel;
                _prevSysUser = sysUser;

                _lastRun = DateTime.UtcNow;

                cpuCopy = _cpuUsage;
            }
            Interlocked.Decrement(ref _runCount);

            return (float)Math.Round(cpuCopy, 2);
        }

        private ulong SubtractTimes(FILETIME a, FILETIME b)
        {
            var aInt = (ulong)(a.dwHighDateTime << 32) | (ulong)a.dwLowDateTime;
            var bInt = (ulong)(b.dwHighDateTime << 32) | (ulong)b.dwLowDateTime;

            return aInt - bInt;
        }

        private bool EnoughTimePassed
        {
            get
            {
                const int minimumElapsedMs = 250;
                var sinceLast = DateTime.UtcNow - _lastRun;
                return sinceLast.TotalMilliseconds > minimumElapsedMs;
            }
        }

        private bool IsFirstRun => _lastRun == DateTime.MinValue;
    }
}