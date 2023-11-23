import { Container, Row, Col } from 'reactstrap';
import { useState, useEffect } from "react";
import { HubConnectionBuilder, LogLevel} from "@microsoft/signalr";

function Dashboard() {
    const MessageTypeValue="VALUE";
    const MessageTypeAverage = "AVERAGE";
    function handleMessage(message, messageType) {
        const updatedSensorsData = sensorsData;
        
        let messageData = message.split(",");
        let singleSensorData;
        if (messageType === MessageTypeValue) {
            singleSensorData = {name: messageData[0], value: messageData[1], unit: messageData[2], mean: "-"};
        } else {
            singleSensorData = {name: messageData[0], value: "-", unit: messageData[2], mean: messageData[1]};
        }
        
        let foundIndex = updatedSensorsData.findIndex(data => data.name === singleSensorData.name);
      
        if (foundIndex === -1) {
            updatedSensorsData.push(singleSensorData)
        } else {
            if (messageType === MessageTypeValue)
                updatedSensorsData[foundIndex].value = singleSensorData.value;
            else
                updatedSensorsData[foundIndex].mean = singleSensorData.mean;
        }
        setSensorsData(updatedSensorsData);
        console.log(sensorsData);
        setIsLoading(false);
    }
    
    
    const [isLoading, setIsLoading] = useState(true);
    const [sensorsData, setSensorsData] = useState([]);
    const [valueMessage, setValueMessage] = useState("");
    const [averageMessage, setAverageMessage] = useState("");
    
    useEffect(() => {
        const connection = new HubConnectionBuilder()
            .withUrl( process.env.REACT_APP_BACKEND_URL +'/sensorhub')
            .withAutomaticReconnect()
            .build();
        connection.on("SendSensorValue", (message) => {
            setValueMessage(message);
        });
        connection.on("SendAverageValue", (message) => {
            setAverageMessage(message);
        })
        connection.start();

        return () => connection.stop();
    }, []);

    useEffect(() => {
        if (valueMessage !== "")
            handleMessage(valueMessage, MessageTypeValue);
    }, [valueMessage]);

    useEffect(() => {
        if (averageMessage !== "")
            handleMessage(averageMessage, MessageTypeAverage);
    }, [averageMessage]);
    
    function sensorsValuesTable() {
        return (
            <table className="table table-striped" aria-labelledby="tableLabel">
                <thead>
                <tr>
                    <th>Name</th>
                    <th>Value</th>
                    <th>Mean</th>
                    <th>Unit</th>
                </tr>
                </thead>
                <tbody>
                {sensorsData.map(sensorData =>
                    <tr key={sensorData.name}>
                        <td>{sensorData.name}</td>
                        <td>{sensorData.value}</td>
                        <td>{sensorData.mean}</td>
                        <td>{sensorData.unit}</td>
                    </tr>
                )}
                </tbody>
            </table>
        );
    }

    return (
        <Container>
            <Row>
                <Col>
                    <h1 id="tableLabel">Dashboard</h1>
                    <p>Data from 4 types of sensors are presented in the table below:</p>
                </Col>
            </Row>
            <Row>
                <Col>
                    <h2 id="secondTableLabel">Data:</h2>
                    {isLoading
                        ? <p><em>Loading...</em></p>
                        : <div>
                            <div className="table-responsive">{sensorsValuesTable()}</div>
                        </div>
                    }
                </Col>
            </Row>
        </Container>
    );
}

export default Dashboard;