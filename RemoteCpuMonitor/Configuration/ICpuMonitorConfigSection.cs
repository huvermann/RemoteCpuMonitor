namespace RemoteCpuMonitor.Configuration
{
    public interface ICpuMonitorConfigSection
    {
        CpuMonitorConfigSection.HostCollection Hosts { get; }
    }
}