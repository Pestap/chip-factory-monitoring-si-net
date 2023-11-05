// See https://aka.ms/new-console-template for more information

using System.Security.AccessControl;
using ChipFactorySimulator.Sensors;
using System;
using ChipFactorySimulator.Utils;

Environment.SetEnvironmentVariable("TEMP_SENSOR_VALUE_SINET", "20");


List<ISensor> sensors = new List<ISensor>();
sensors = FileUtils.ReadSensorsFromFile("D:\\Piotrek\\Semestr 7\\SINET\\chip-factory-monitoring-si-net\\ChipFactorySimulator\\sensors.csv");
/*// Temperature sensors

ISensor t1 = new TemperatureSensor("t1",22, 25, 12);
ISensor t2 = new TemperatureSensor("t2", 15, 17, 60);
ISensor t3 = new TemperatureSensor("t3", 45, 70, 12);
ISensor t4 = new TemperatureSensor("t4", 0, 3, 60);
sensors.Add(t1);
sensors.Add(t2);
sensors.Add(t3);
sensors.Add(t4);

// Humidity sensors

ISensor h1 = new HumiditySensor("h1",80, 100, 20);
ISensor h2 = new HumiditySensor("h2", 45, 60, 30);
ISensor h3 = new HumiditySensor("h3", 45, 70, 12);
ISensor h4 = new HumiditySensor("h4", 1, 3, 60);

sensors.Add(h1);
sensors.Add(h2);
sensors.Add(h3);
sensors.Add(h4);

// Dust sensors

ISensor d1 = new DustSensor("d1", 350, 450, 20);
ISensor d2 = new DustSensor("d2", 100, 200, 3);
ISensor d3 = new DustSensor("d3", 0, 100, 12);
ISensor d4 = new DustSensor("d4", 125, 500, 6);

sensors.Add(d1);
sensors.Add(d2);
sensors.Add(d3);
sensors.Add(d4);

// Airflow sensors

ISensor a1 = new AirflowSensor("a1",350, 450, 25);
ISensor a2 = new AirflowSensor("a2", 100, 200, 40);
ISensor a3 = new AirflowSensor("a3",0, 100, 10);
ISensor a4 = new AirflowSensor("a4", 125, 500, 60);

sensors.Add(a1);
sensors.Add(a2);
sensors.Add(a3);
sensors.Add(a4);
*/

// for each sensor start thread

foreach (ISensor sensor in sensors)
{
    Thread t = new Thread(sensor.StartGenerating);
    t.Start();
}





