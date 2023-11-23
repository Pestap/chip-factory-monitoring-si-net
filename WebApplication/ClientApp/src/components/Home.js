import React, { Component } from 'react';

export class Home extends Component {
  static displayName = Home.name;

  render() {
    return (
      <div>
        <h1>Web application for .NET web services course</h1>
        <p>This application was created by:</p>
        <ul>
            <li>Piotr Pesta</li>
            <li>Bartłomiej Szczepaniec</li>
        </ul>
      </div>
    );
  }
}
