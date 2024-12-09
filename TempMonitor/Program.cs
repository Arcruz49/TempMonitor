using Spectre.Console;
using LibreHardwareMonitor.Hardware;
using System.Management;


class Program
{
    private static Computer computer;
    private static Table table = new Table();

    static void Main(string[] args)
    {
        OnStart();
        try
        {
            var leftContent = "";
            var rightContent = "";
            while (true)
            {
                AnsiConsole.Clear();
                table.Rows.Clear();
                leftContent = "";
                rightContent = "";
                foreach (var hardware in computer.Hardware)
                {

                    hardware.Update();

                    if (HardwareValidType(hardware.HardwareType))
                    {
                        leftContent += $"[bold yellow]{hardware.Name}[/]\n";
                        

                        foreach (var sensor in hardware.Sensors)
                        {
                            if (sensor.Value != null && !float.IsNaN(sensor.Value.Value))
                            {
                                leftContent += HandleSensorDisplay(sensor);
                            }
                        }
                    }
                    leftContent += "\n";
                }
                rightContent = RamUsage();
                table.AddRow(leftContent, rightContent);

                AnsiConsole.Write(table);

                AnsiConsole.MarkupLine("[gray]Loading in 2 seconds...[/]");
                Thread.Sleep(2000);
            }
        }
        catch (Exception ex)
        {
            OnStop(ex);
        }
    }

    static string HandleSensorDisplay(ISensor sensor)
    {
        const int typeColumnWidth = 45;
        const int valueColumnWidth = 15;

        switch (sensor.SensorType)
        {
            case SensorType.Voltage:
                return $"  [blue]{"Voltage - " + sensor.Name,-typeColumnWidth}[/]: [green]{sensor.Value,-valueColumnWidth} V[/]\n";

            case SensorType.Current:
                return$"  [blue]{"Current - " + sensor.Name,-typeColumnWidth}[/]: [green]{sensor.Value,-valueColumnWidth} A[/]\n";

            case SensorType.Power:
                return $"  [blue]{"Power - " + sensor.Name,-typeColumnWidth}[/]: [green]{sensor.Value,-valueColumnWidth} W[/]\n";

            case SensorType.Clock:
                return $"  [blue]{"Clock - " + sensor.Name,-typeColumnWidth}[/]: [green]{sensor.Value,-valueColumnWidth} MHz[/]\n";
                 
            case SensorType.Temperature:
                return $"  [blue]{"Temperature - " + sensor.Name,-typeColumnWidth}[/]: [green]{sensor.Value,-valueColumnWidth} \u00b0 C[/]\n";
                 
            case SensorType.Load:
                return $"  [blue]{"Load - " + sensor.Name,-typeColumnWidth}[/]: [green]{sensor.Value,-valueColumnWidth} %[/]\n";
                 
            case SensorType.Frequency:
                return $"  [blue]{"Frequency - " + sensor.Name,-typeColumnWidth}[/]: [green]{sensor.Value,-valueColumnWidth} MHz[/]\n";
                 
            case SensorType.Fan:
                return $"  [blue]{"Fan - " + sensor.Name,-typeColumnWidth}[/]: [green]{sensor.Value,-valueColumnWidth} RPM[/]\n";
                 
            case SensorType.Flow:
                return $"  [blue]{"Flow - " + sensor.Name,-typeColumnWidth}[/]: [green]{sensor.Value,-valueColumnWidth} L/min[/]\n";
                 
            case SensorType.Level:
                return $"  [blue]{"Level - " + sensor.Name,-typeColumnWidth}[/]: [green]{sensor.Value,-valueColumnWidth} %[/]\n";
                 
            case SensorType.Energy:
                return $"  [blue]{"Energy - " + sensor.Name,-typeColumnWidth}[/]: [green]{sensor.Value,-valueColumnWidth} J[/]\n";
                 
            case SensorType.Humidity:
                return $"  [blue]{"Humidity - " + sensor.Name,-typeColumnWidth}[/]: [green]{sensor.Value,-valueColumnWidth} %[/]\n";
                 
            case SensorType.Noise:
                return $"  [blue]{"Noise - " + sensor.Name,-typeColumnWidth}[/]: [green]{sensor.Value,-valueColumnWidth} dB[/]\n";
                 
            case SensorType.Conductivity:
                return $"  [blue]{"Conductivity - " + sensor.Name,-typeColumnWidth}[/]: [green]{sensor.Value,-valueColumnWidth} μS/cm[/]\n";

            case SensorType.TimeSpan:
                return $"  [blue]{"TimeSpan - " + sensor.Name,-typeColumnWidth}[/]: [green]{sensor.Value,-valueColumnWidth} ms[/]\n";

            case SensorType.SmallData:
                return $"  [blue]{"SmallData - " + sensor.Name,-typeColumnWidth}[/]: [green]{sensor.Value,-valueColumnWidth} MB[/]\n";

            default:
                return $"  [blue]{"Unknown - " + sensor.Name,-typeColumnWidth}[/]: [green]{sensor.Value,-valueColumnWidth}[/]\n";
                 
        }
    }

    static void OnStart()
    {
        computer = new Computer
        {
            IsCpuEnabled = true,
            IsGpuEnabled = true,
            IsStorageEnabled = false,
            IsMotherboardEnabled = false
        };

        table.Expand();
        table.AddColumn("");
        table.AddColumn("");
        table.HideHeaders();



        computer.Open();
        AnsiConsole.MarkupLine("[green]Starting...[/]");
        Thread.Sleep(2000);

    }

    static void OnStop(Exception ex)
    {
        AnsiConsole.MarkupLine($"[red]Error: {ex.Message}[/]");
        AnsiConsole.MarkupLine("[red]Finalizando sistema de monitoramento...[/]");
        if (computer != null)
        {
            computer.Close();
            computer = null;
        }
    }

    static bool HardwareValidType(HardwareType hardwareType)
    {
        return hardwareType == HardwareType.Motherboard ||
            hardwareType == HardwareType.Cpu ||
            hardwareType == HardwareType.Memory ||
            hardwareType == HardwareType.GpuNvidia ||
            hardwareType == HardwareType.GpuAmd ||
            hardwareType == HardwareType.GpuIntel ||
            hardwareType == HardwareType.Storage ||
            hardwareType == HardwareType.Battery;
    }


    static string RamUsage()
    {
        var searcher = new ManagementObjectSearcher("SELECT * FROM Win32_OperatingSystem");
        var content = "";

        foreach (var obj in searcher.Get())
        {
            double totalMemoryGB = Convert.ToDouble(obj["TotalVisibleMemorySize"]) / 1024 / 1024;
            double freeMemoryGB = Convert.ToDouble(obj["FreePhysicalMemory"]) / 1024 / 1024;
            double usedMemoryGB = totalMemoryGB - freeMemoryGB;

            double usagePercentage = (usedMemoryGB / totalMemoryGB) * 100;

            content = $"Total Memory: {totalMemoryGB:F2} GB\n";
            content += $"Free Memory: {freeMemoryGB:F2} GB\n";
            content += $"Used Memory: {usedMemoryGB:F2} GB\n";
            content += $"Memory Usage: {usagePercentage:F2}%\n";

            content +=(
            new BarChart()
                .Width(60)
                .Label("[bold blue]Memory Usage[/]")
                .AddItem("Used", (float)usagePercentage, Color.Red)
                .AddItem("Free", (float)(100 - usagePercentage), Color.Green)
            ).ToString();

        }


        return content;
    }
}
