import {Button, Container, Row, Col, Input, Label, Form, FormGroup} from 'reactstrap';
import { useState, useEffect } from "react";
import {Chart} from "react-google-charts";

function Sensors() {
    
    function changeDateToISOFormatUTC(date) {
        let dateISO = date;
        if (dateISO !== "") {
            dateISO += ":00Z";
        }
        return dateISO;
    }
    
    async function handleFormSubmit(event) {
        event.preventDefault();
        
        setAppliedDateTo(changeDateToISOFormatUTC(chosenDateTo));
        setAppliedDateFrom(changeDateToISOFormatUTC(chosenDateFrom));
        
        let typeFilters = "";
        chosenTypeFilters.forEach(chosenSortFilter => {
            typeFilters += chosenSortFilter;
            typeFilters += ",";
        })
        if (typeFilters.length > 0)
            typeFilters = typeFilters.substring(0, typeFilters.length - 1);
        setAppliedTypeFilters(typeFilters);

        let nameFilters = "";
        chosenNameFilters.forEach(chosenNameFilter => {
            nameFilters += chosenNameFilter;
            nameFilters += ",";
        })
        if (nameFilters.length > 0)
            nameFilters = nameFilters.substring(0, nameFilters.length - 1);
        setAppliedNameFilters(nameFilters);
        
        console.log("TYPEFILTERS=" + typeFilters);
        console.log("NAMEFILTERS=" + nameFilters);
        console.log("SUBMITED" + appliedDateFrom + " " + chosenDateTo);
    }

    async function handleSensorTypeFilterChange(type, event) {
        console.log(type + event.target.checked);
        let typeFilters = [...chosenTypeFilters];
        if(event.target.checked) {
            typeFilters.push(type);
        } else {
            typeFilters = typeFilters.filter(typeFilter => typeFilter !== type)
        }
        console.log(typeFilters);
        setChosenTypeFilters(typeFilters);
    }

    async function handleSensorNameFilterChange(name, event) {
        console.log(name + event.target.checked);
        let nameFilters = [...chosenNameFilters];
        if(event.target.checked) {
            nameFilters.push(name);
        } else {
            nameFilters = nameFilters.filter(typeFilter => typeFilter !== name)
        }
        console.log(nameFilters);
        setChosenNameFilters(nameFilters);
    }
    
    const [isLoading, setIsLoading] = useState(true);
    const [areFiltersLoading, setAreFiltersLoading] = useState(true);
    const [areSortTypesLoading, setAreSortTypesLoading] = useState(true);
    const [sensorsData, setSensorsData] = useState([]);
    const [sortTypes, setSortTypes] = useState({});
    
    const [sensorNames, setSensorNames] = useState([]);
    const [sensorTypes, setSensorTypes] = useState([]);

    const [chosenDateFrom, setChosenDateFrom] = useState("");
    const [chosenDateTo, setChosenDateTo] = useState("");
    const [chosenTypeFilters, setChosenTypeFilters] = useState([]);
    const [chosenNameFilters, setChosenNameFilters] = useState([]);

    const [chosenSortType, setChosenSortType] = useState("");
    
    const [appliedDateFrom, setAppliedDateFrom] = useState("");
    const [appliedDateTo, setAppliedDateTo] = useState("");
    const [appliedTypeFilters, setAppliedTypeFilters] = useState("");
    const [appliedNameFilters, setAppliedNameFilters] = useState("");

    const [chosenQueryParams, setChosenQueryParams] = useState("");

    const [filteredChartData, setFilteredChartData] = useState([]);
    const [chartOptions, setOptions] = useState({
        title: "Sensors Data",
        legend: { position: "bottom" }
    });
    
    useEffect(() => {
        let resource = `${process.env.REACT_APP_BACKEND_URL}/api/sensors`;
        
        let queryParams = "";
        if (chosenSortType !== "" || appliedTypeFilters !== "" || appliedNameFilters !== "" || appliedDateTo !== "" || appliedDateFrom !== "")
            queryParams+="?"
        if (chosenSortType !== "") {
            if(queryParams.length > 1) {
                queryParams += "&";
            }
            queryParams += "sort-by=" + chosenSortType;
        }
        if ( appliedTypeFilters !== "") {
            if(queryParams.length > 1) {
                queryParams += "&";
            }
            queryParams += "type=" + appliedTypeFilters;
        }
        if ( appliedNameFilters !== "") {
            if(queryParams.length > 1) {
                queryParams += "&";
            }
            queryParams += "name=" + appliedNameFilters;
        }
        if ( appliedDateTo !== "") {
            if(queryParams.length > 1) {
                queryParams += "&";
            }
            queryParams += "dateTo=" + appliedDateTo;
        }
        if ( appliedDateFrom !== "") {
            if(queryParams.length > 1) {
                queryParams += "&";
            }
            queryParams += "dateFrom=" + appliedDateFrom;
        }
        resource += queryParams;
        setChosenQueryParams(queryParams);
        fetch(resource)
            .then(response => response.json())
            .then(data => {
                setSensorsData(data);
                setIsLoading(false);
            })
            .catch(error => console.error(error));
    }, [chosenSortType, appliedDateFrom, appliedDateTo, appliedTypeFilters, appliedNameFilters]);

    // Fetching data for chart presentation
    useEffect(() => {
        let resource = `${process.env.REACT_APP_BACKEND_URL}/api/sensors`;

        let queryParams = "";

        queryParams += "?sort-by=SortByDateAsc"
        
        if ( appliedTypeFilters !== "") {
            if(queryParams.length > 1) {
                queryParams += "&";
            }
            queryParams += "type=" + appliedTypeFilters;
        }
        if ( appliedNameFilters !== "") {
            if(queryParams.length > 1) {
                queryParams += "&";
            }
            queryParams += "name=" + appliedNameFilters;
        }
        if ( appliedDateTo !== "") {
            if(queryParams.length > 1) {
                queryParams += "&";
            }
            queryParams += "dateTo=" + appliedDateTo;
        }
        if ( appliedDateFrom !== "") {
            if(queryParams.length > 1) {
                queryParams += "&";
            }
            queryParams += "dateFrom=" + appliedDateFrom;
        }
        resource += queryParams;
        
        fetch(resource)
            .then(response => response.json())
            .then(data => {
                console.log("CHART DATA");
                console.log(data);
                
                const availableSensors = [];
                data.forEach(sensorData => {
                    if (!availableSensors.includes(sensorData.name)) {
                        availableSensors.push(sensorData.name);
                    }
                })
                availableSensors.sort();
                const firstRow = ["Time", ...availableSensors];
                console.log(firstRow);
                
                const chartData = [];
                chartData.push(firstRow);
                
                let chartDataRow = [];
                
                for (let i = 0; i < firstRow.length; i++) {
                    chartDataRow.push(null);
                }
                data.forEach(sensorData => {
                   if (chartDataRow[0] === null) {
                       chartDataRow[0] = sensorData.time;
                       chartDataRow[firstRow.findIndex(name => name === sensorData.name)] = sensorData.value;
                   } else if (chartDataRow[0] === sensorData.time) {
                       chartDataRow[firstRow.findIndex(name => name === sensorData.name)] = sensorData.value;
                   } else {
                       chartData.push(chartDataRow);
                       chartDataRow = [];
                       for (let i = 0; i < firstRow.length; i++) {
                           chartDataRow.push(null);
                       }
                       chartDataRow[0] = sensorData.time;
                       chartDataRow[firstRow.findIndex(name => name === sensorData.name)] = sensorData.value;
                   }
                });
                chartData.push(chartDataRow);
                setFilteredChartData(chartData);
            })
            .catch(error => console.error(error));
    }, [appliedDateFrom, appliedDateTo, appliedTypeFilters, appliedNameFilters]);

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
                                    onChange={event => setChosenDateFrom(event.target.value)}
                                />
                            </FormGroup>
                            <FormGroup>
                                <Label>Filter readings to date:</Label>
                                <Input
                                    id="datetime-to"
                                    name="datetime-to"
                                    type="datetime-local"
                                    onChange={event => setChosenDateTo(event.target.value)}
                                />
                            </FormGroup>
                            <FormGroup>
                                <Label>Filter by sensor's type:</Label>
                                {sensorTypes.map(sensorType => <div key={sensorType}>
                                    <Input className="pg-checkbox"
                                           name="sensor-type"
                                           type="checkbox"
                                           onChange={event => handleSensorTypeFilterChange(sensorType, event)}
                                    />
                                    <Label check>
                                        {sensorType}
                                    </Label>
                                </div>)}
                            </FormGroup>
                            <FormGroup>
                                <Label>Filter by sensor's name:</Label>
                                {sensorNames.map(sensorName => <div key={sensorName}>
                                    <Input className="pg-checkbox"
                                           name="sensor-name"
                                           type="checkbox"
                                           onChange={event => handleSensorNameFilterChange(sensorName, event)}
                                    />
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
                    {filteredChartData.length > 1 && <div>
                        <Chart
                            chartType="ScatterChart"
                            width="100%"
                            height="400px"
                            data={filteredChartData}
                            options={chartOptions}
                        />
                    </div>}
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
                                            href={`${process.env.REACT_APP_BACKEND_URL}/api/sensors/csv${chosenQueryParams}`}
                                            tag="a"
                                            className="pg-btn-download"
                                        >
                                            Pobierz do csv
                                        </Button>
                                        <Button
                                            color="primary"
                                            href={`${process.env.REACT_APP_BACKEND_URL}/api/sensors/json${chosenQueryParams}`}
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