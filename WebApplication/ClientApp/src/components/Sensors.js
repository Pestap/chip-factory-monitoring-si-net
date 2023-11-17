import {Container, Row, Col, Input, Label} from 'reactstrap';
import { useState, useEffect } from "react";

function Sensors() {
    const [isLoading, setIsLoading] = useState(true);
    const [sensorsData, setSensorsData] = useState([]);
    const [sortTypes, setSortTypes] = useState({});
    
    const [chosenSortType, setChosenSortType] = useState("");

    useEffect(() => {
        
        let resource = 'http://localhost:80/api/sensors';
        if ( chosenSortType !== "") {
            resource+= "?sort-by=" + chosenSortType
        }
        console.log(resource);
        fetch(resource)
            .then(response => response.json())
            .then(data => {
                setSensorsData(data);
                setIsLoading(false);
            })
            .catch(error => console.error(error));
        fetch('http://localhost:80/api/sensors/sort-types')
            .then(response => response.json())
            .then(data => {
                setSortTypes(data);
            })
            .catch(error => console.error(error));
    }, [chosenSortType]);
    
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
                    <tr key={sensorData.id}>
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
                        : <div>
                            <Row>
                                <Col xs={4}>
                                    <Label for="exampleSelect">Sort by:</Label>
                                    <Input
                                        id="exampleSelect"
                                        name="select"
                                        type="select"
                                        onChange={e => setChosenSortType(e.target.value)}
                                    >
                                        {Object.keys(sortTypes).map( sortTypeKey =>
                                            <option key={sortTypeKey} value={sortTypeKey}>
                                                {sortTypes[sortTypeKey]}
                                            </option>
                                        )}
                                    </Input>
                                </Col>
                            </Row>
                            <div className="table-responsive">{sensorsValuesTable()}</div>
                        </div>
                    }
                </Col>
            </Row>
        </Container>
    );
}

export default Sensors;