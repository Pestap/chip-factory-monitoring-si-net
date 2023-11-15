import { Container, Row, Col } from 'reactstrap';
import { useState, useEffect } from "react";

function Sensors() {
    const [isLoading, setIsLoading] = useState(true);
    const [sensorsData, setSensorsData] = useState([]);

    useEffect(() => {
        fetch('api/sensors')
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
                    <th>Type</th>
                    <th>Name</th>
                    <th>Value</th>
                    <th>Unit</th>
                    <th>Time</th>
                </tr>
                </thead>
                <tbody>
                {sensorsData.map(sensorData =>
                    <tr key={sensorData.time}>
                        <td>{sensorData.topic}</td>
                        <td>{sensorData.name}</td>
                        <td>{sensorData.value}</td>
                        <td>{sensorData.unitOfMeasurement}</td>
                        <td>{sensorData.time}</td>
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
                    <h1 id="tableLabel">Sensors data</h1>
                    <p>Data from 4 types of sensors are presented in the table below:</p>
                </Col>
            </Row>
            <Row>
                <Col xs={3}>
                    <h2>Filters:</h2>
                </Col>
                <Col xs={9}>
                    <h2 id="secondTableLabel">Data:</h2>
                    {isLoading 
                        ? <p><em>Loading...</em></p> 
                        : <div className="table-responsive">{sensorsValuesTable()}</div>
                    }
                </Col>
            </Row>
        </Container>
    );
}

export default Sensors;