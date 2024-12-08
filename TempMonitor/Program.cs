using Spectre.Console;
using LibreHardwareMonitor.Hardware;


class Program
{
    private static Computer computer;

    static void Main(string[] args)
    {
        OnStart();
        try
        {
            while (true)
            {
                AnsiConsole.Clear();

                foreach (var hardware in computer.Hardware)
                {
                    hardware.Update();

                    if (HardwareValidType(hardware.HardwareType))
                    {
                        AnsiConsole.MarkupLine($"[bold yellow]{hardware.Name}[/]");

                        foreach (var sensor in hardware.Sensors)
                        {
                            if (sensor.Value != null && !float.IsNaN(sensor.Value.Value))
                            {
                                HandleSensorDisplay(sensor);
                            }
                        }

                        AnsiConsole.WriteLine();
                    }
                }

                AnsiConsole.MarkupLine("[gray]Loading in 2 seconds...[/]");
                Thread.Sleep(2000);
            }
        }
        catch (Exception ex)
        {
            OnStop(ex);
        }
        
    }

    static void HandleSensorDisplay(ISensor sensor)
    {
        const int typeColumnWidth = 45;
        const int valueColumnWidth = 15;

        switch (sensor.SensorType)
        {
            case SensorType.Voltage:
                AnsiConsole.MarkupLine($"  [blue]{"Voltage - " + sensor.Name,-typeColumnWidth}[/]: [green]{sensor.Value,-valueColumnWidth} V[/]");
                break;
            case SensorType.Current:
                AnsiConsole.MarkupLine($"  [blue]{"Current - " + sensor.Name,-typeColumnWidth}[/]: [green]{sensor.Value,-valueColumnWidth} A[/]");
                break;
            case SensorType.Power:
                AnsiConsole.MarkupLine($"  [blue]{"Power - " + sensor.Name,-typeColumnWidth}[/]: [green]{sensor.Value,-valueColumnWidth} W[/]");
                break;
            case SensorType.Clock:
                AnsiConsole.MarkupLine($"  [blue]{"Clock - " + sensor.Name,-typeColumnWidth}[/]: [green]{sensor.Value,-valueColumnWidth} MHz[/]");
                break;
            case SensorType.Temperature:
                AnsiConsole.MarkupLine($"  [blue]{"Temperature - " + sensor.Name,-typeColumnWidth}[/]: [green]{sensor.Value,-valueColumnWidth} \u00b0 C[/]");
                break;
            case SensorType.Load:
                AnsiConsole.MarkupLine($"  [blue]{"Load - " + sensor.Name,-typeColumnWidth}[/]: [green]{sensor.Value,-valueColumnWidth} %[/]");
                break;
            case SensorType.Frequency:
                AnsiConsole.MarkupLine($"  [blue]{"Frequency - " + sensor.Name,-typeColumnWidth}[/]: [green]{sensor.Value,-valueColumnWidth} MHz[/]");
                break;
            case SensorType.Fan:
                AnsiConsole.MarkupLine($"  [blue]{"Fan - " + sensor.Name,-typeColumnWidth}[/]: [green]{sensor.Value,-valueColumnWidth} RPM[/]");
                break;
            case SensorType.Flow:
                AnsiConsole.MarkupLine($"  [blue]{"Flow - " + sensor.Name,-typeColumnWidth}[/]: [green]{sensor.Value,-valueColumnWidth} L/min[/]");
                break;
            case SensorType.Level:
                AnsiConsole.MarkupLine($"  [blue]{"Level - " + sensor.Name,-typeColumnWidth}[/]: [green]{sensor.Value,-valueColumnWidth} %[/]");
                break;
            case SensorType.Energy:
                AnsiConsole.MarkupLine($"  [blue]{"Energy - " + sensor.Name,-typeColumnWidth}[/]: [green]{sensor.Value,-valueColumnWidth} J[/]");
                break;
            case SensorType.Humidity:
                AnsiConsole.MarkupLine($"  [blue]{"Humidity - " + sensor.Name,-typeColumnWidth}[/]: [green]{sensor.Value,-valueColumnWidth} %[/]");
                break;
            case SensorType.Noise:
                AnsiConsole.MarkupLine($"  [blue]{"Noise - " + sensor.Name,-typeColumnWidth}[/]: [green]{sensor.Value,-valueColumnWidth} dB[/]");
                break;
            case SensorType.Conductivity:
                AnsiConsole.MarkupLine($"  [blue]{"Conductivity - " + sensor.Name,-typeColumnWidth}[/]: [green]{sensor.Value,-valueColumnWidth} μS/cm[/]");
                break;
            case SensorType.TimeSpan:
                AnsiConsole.MarkupLine($"  [blue]{"TimeSpan - " + sensor.Name,-typeColumnWidth}[/]: [green]{sensor.Value,-valueColumnWidth} ms[/]");
                break;
            case SensorType.SmallData:
                AnsiConsole.MarkupLine($"  [blue]{"SmallData - " + sensor.Name,-typeColumnWidth}[/]: [green]{sensor.Value,-valueColumnWidth} MB[/]");
                break;
            default:
                AnsiConsole.MarkupLine($"  [blue]{"Unknown - " + sensor.Name,-typeColumnWidth}[/]: [green]{sensor.Value,-valueColumnWidth}[/]");
                break;
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
}
