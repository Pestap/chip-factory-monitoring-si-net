using System.Globalization;
using System.Security.AccessControl;
using ChipFactorySimulator.Sensors;
using Microsoft.VisualBasic.FileIO;

namespace ChipFactorySimulator.Utils;

public class FileUtils
{
    public static List<ISensor> ReadSensorsFromFile(string filepath)
    {
        var sensors = new List<ISensor>();

        using (StreamReader sr = new StreamReader(filepath))
        {
            NumberFormatInfo provider = new NumberFormatInfo();
            provider.NumberDecimalSeparator = ".";
            provider.NumberGroupSeparator = ",";
            
            sr.ReadLine();
            string currentLine;
            while((currentLine = sr.ReadLine()) != "" && currentLine != null)
            {
                var values = currentLine.Split(";");
                var type = values[0];
                var name = values[1];
                var from = Convert.ToDouble(values[2], provider);
                var to = Convert.ToDouble(values[3], provider);
                var interval = Convert.ToDouble(values[4], provider);
                var israndom = Convert.ToBoolean(values[5]);

                switch (type)
                {
                    case "temperature":
                        sensors.Add(new TemperatureSensor(name, from,to, interval));
                        break;
                    case "humidity":
                        sensors.Add(new HumiditySensor(name, from,to, interval));
                        break;
                    case "dust":
                        sensors.Add(new DustSensor(name, from,to, interval));
                        break;
                    case "airflow":
                        sensors.Add(new AirflowSensor(name, from,to, interval));
                        break;
                    default:
                        break;
                }
            }
        }

        return sensors;
    }
}