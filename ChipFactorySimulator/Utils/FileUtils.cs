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
                var values = currentLine.Split(",");
                var type = values[0];
                var name = values[1];
                var from = Convert.ToDouble(values[2], provider);
                var to = Convert.ToDouble(values[3], provider);
                var interval = Convert.ToDouble(values[4], provider);
                var israndom = Convert.ToBoolean(values[5]);
                var setValue = 0.0;
                if (!israndom)
                {
                    setValue = Convert.ToDouble(values[6], provider);
                }

                ISensor sensor = null;
                
                switch (type)
                {
                    case "temperature":
                        sensor = new TemperatureSensor(name, from,to, interval, israndom);
                        break;
                    case "humidity":
                        sensor = new HumiditySensor(name, from,to, interval, israndom);
                        break;
                    case "dust":
                        sensor = new DustSensor(name, from,to, interval, israndom);
                        break;
                    case "airflow":
                        sensor = new AirflowSensor(name, from,to, interval, israndom);
                        break;
                }

                sensor.GeneratedSetValue = setValue;
                
                sensors.Add(sensor);

            }
        }

        return sensors;
    }
}