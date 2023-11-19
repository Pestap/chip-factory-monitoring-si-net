import {Button, Container, Row, Col, Input, Label, Form, FormGroup} from 'reactstrap';
import { useState, useEffect } from "react";
import {Await} from "react-router-dom";


function Sensors() {
    async function handleFormSubmit(event) {
        event.preventDefault();
        
        console.log("SUBMITED" + dateFrom + " " + dateTo);
    }
    
    const [isLoading, setIsLoading] = useState(true);
    const [areFiltersLoading, setAreFiltersLoading] = useState(true);
    const [areSortTypesLoading, setAreSortTypesLoading] = useState(true);
    const [sensorsData, setSensorsData] = useState([]);
    const [sortTypes, setSortTypes] = useState({});
    
    const [dateFrom, setDateFrom] = useState("");
    const [dateTo, setDateTo] = useState("");
    const [sensorNames, setSensorNames] = useState([]);
    const [sensorTypes, setSensorTypes] = useState([]);

    const [chosenSortType, setChosenSortType] = useState("");

    useEffect(() => {
        
        let resource = `${process.env.REACT_APP_BACKEND_URL}/api/sensors`;
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
    }, [chosenSortType]);

    useEffect(() => {
        fetch(`${process.env.REACT_APP_BACKEND_URL}/api/sensors/sort-types`)
            .then(response => response.json())
            .then(data => {
                setSortTypes(data);
                setAreSortTypesLoading(false);
            })
            .catch(error => console.error(error));

        let promiseFilters1 = fetch(`${process.env.REACT_APP_BACKEND_URL}/api/sensors/sensors-types`)
            .then(response => response.json())
            .then(data => {
                setSensorTypes(data);
            })
            .catch(error => console.error(error));

        let promiseFilters2 = fetch(`${process.env.REACT_APP_BACKEND_URL}/api/sensors/sensors-names`)
            .then(response => response.json())
            .then(data => {
                setSensorNames(data);
            })
            .catch(error => console.error(error));
        
        Promise.all([promiseFilters1, promiseFilters2])
            .then(r => setAreFiltersLoading(false));
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
                    {areFiltersLoading
                        ? <p><em>Loading...</em></p>
                        : <Form onSubmit={handleFormSubmit}>
                            <FormGroup>
                                <Button color="primary">Apply filters</Button>
                            </FormGroup>
                            <FormGroup>
                                <Label>Filter readings after date:</Label>
                                <Input
                                    id="datetime-from"
                                    name="datetime-from"
                                    type="datetime-local"
                                    onChange={event => setDateFrom(event.target.value)}
                                />
                            </FormGroup>
                            <FormGroup>
                                <Label>Filter readings to date:</Label>
                                <Input
                                    id="datetime-to"
                                    name="datetime-to"
                                    type="datetime-local"
                                    onChange={event => setDateTo(event.target.value)}
                                />
                            </FormGroup>
                            <FormGroup>
                                <Label>Filter by sensor's type:</Label>
                                {sensorTypes.map(sensorType => <div key={sensorType}>
                                    <Input className="pg-checkbox" name="sensor-type" type="checkbox" />
                                    <Label check>
                                        {sensorType}
                                    </Label>
                                </div>)}
                            </FormGroup>
                            <FormGroup>
                                <Label>Filter by sensor's name:</Label>
                                {sensorNames.map(sensorName => <div key={sensorName}>
                                    <Input className="pg-checkbox" name="sensor-name" type="checkbox" />
                                    <Label check>
                                        {sensorName}
                                    </Label>
                                </div>)}
                            </FormGroup>
                        </Form>
                    }
                </Col>
                <Col xs={9}>
                    <h2 id="secondTableLabel">Data:</h2>
                    {areSortTypesLoading
                        ? <p><em>Loading...</em></p>
                        : <div>
                            <Row className="align-items-end">
                                <Col xs={4}>
                                    <Label for="exampleSelect">Sort by:</Label>
                                    <Input
                                        id="exampleSelect"
                                        name="select"
                                        type="select"
                                        onChange={e => {setChosenSortType(e.target.value); setIsLoading(false)}}
                                    >
                                        {Object.keys(sortTypes).map( sortTypeKey =>
                                            <option key={sortTypeKey} value={sortTypeKey}>
                                                {sortTypes[sortTypeKey]}
                                            </option>
                                        )}
                                    </Input>
                                </Col>
                                <Col xs={8}>
                                    {!isLoading && <div>
                                        <Button
                                            color="primary"
                                            href={chosenSortType ? `http://localhost/api/sensors/csv?sort-by=${chosenSortType}` : "http://localhost/api/sensors/csv"}
                                            tag="a"
                                            className="pg-btn-download"
                                        >
                                            Pobierz do csv
                                        </Button>
                                        <Button
                                            color="primary"
                                            href={chosenSortType ? `http://localhost/api/sensors/json?sort-by=${chosenSortType}` : "http://localhost/api/sensors/json"}
                                            tag="a"
                                            className="pg-btn-download"
                                        >
                                            Pobierz do json
                                        </Button>
                                    </div>}
                                </Col>
                            </Row>
                            {isLoading
                                ? <p><em>Loading...</em></p>
                                : <div className="table-responsive">{sensorsValuesTable()}</div>
                            }
                        </div>
                    }
                </Col>
            </Row>
        </Container>
    );
}

export default Sensors;