import { Container, Row, Col } from 'reactstrap';
import { useState, useEffect } from "react";
import useWebSocket from "react-use-websocket";

function Dashboard() {
    const [isLoading, setIsLoading] = useState(true);
    const [sensorsData, setSensorsData] = useState([]);

    const [socketUrl, setSocketUrl] = useState('');

    const { sendMessage, lastMessage, readyState } = useWebSocket(socketUrl);

    useEffect(() => {
        if (lastMessage !== null) {
            console.log(lastMessage);
        }
    }, [lastMessage]);
    

    useEffect(() => {

        let resource = process.env.REACT_APP_BACKEND_URL +'/api/sensors';
        fetch(resource)
            .then(response => response.json())
            .then(data => {
                setSensorsData(data);
                setIsLoading(false);
            })
            .catch(error => console.error(error));
    }, []);

    function sensorsValuesTable() {
        return (
            <table className="table table-striped" aria-labelledby="tableLabel">
                <thead>
                <tr>
                    <th>Name</th>
                    <th>Value</th>
                    <th>Mean</th>
                </tr>
                </thead>
                <tbody>
                {sensorsData.map(sensorData =>
                    <tr key={sensorData.name}>
                        <td>{sensorData.name}</td>
                        <td>{sensorData.value}</td>
                        <td>{sensorData.value}</td>
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