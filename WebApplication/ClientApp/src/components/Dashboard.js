import { Container, Row, Col } from 'reactstrap';
import { useState, useEffect } from "react";
import { HubConnectionBuilder, LogLevel} from "@microsoft/signalr";
import {Await} from "react-router-dom";

function Dashboard() {
    const [isLoading, setIsLoading] = useState(true);
    const [sensorsData, setSensorsData] = useState([]);

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
    useEffect(() => {
        const connection = new HubConnectionBuilder()
            .withUrl( process.env.REACT_APP_BACKEND_URL +'/sensorhub')
            .withAutomaticReconnect()
            .build();
        connection.on("SendSensorValue", (message) => {
            console.log(message);
        });
        connection.start();
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