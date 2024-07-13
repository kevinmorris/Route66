import React, {useCallback, useEffect, useMemo, useState} from 'react';
import ReactDOM from 'react-dom/client';
import './index.css';
import App from './App';
import reportWebVitals from './reportWebVitals';

const data = [
    {
        "id": "3557b7a8-6b69-4dd6-81f5-3ef07d2de7e8",
        "state": "Missouri",
        "color": "Mauv",
        "job": "Software Test Engineer IV"
    }, {
        "id": "a8af6fba-c071-463e-b877-45476e5feaf9",
        "state": "Washington",
        "color": "Pink",
        "job": "Software Engineer I"
    }, {
        "id": "e2180487-6190-4e42-831a-578bce444ebd",
        "state": "California",
        "color": "Mauv",
        "job": "Help Desk Operator"
    }, {
        "id": "55f099e2-219d-464d-8ba1-94c6aa182742",
        "state": "California",
        "color": "Blue",
        "job": "Junior Executive"
    }, {
        "id": "dbb13a75-ba52-4de0-89d6-c71620fb2a4c",
        "state": "District of Columbia",
        "color": "Puce",
        "job": "Recruiting Manager"
    }, {
        "id": "5a18d72e-ef4a-46b4-aedc-3ad5a41eb8df",
        "state": "New York",
        "color": "Pink",
        "job": "Programmer III"
    }, {
        "id": "008570a8-afce-441c-997b-820e2b0365f8",
        "state": "Georgia",
        "color": "Blue",
        "job": "Nurse"
    }, {
        "id": "3e4094d5-a685-4d00-99f2-94275fab876c",
        "state": "Georgia",
        "color": "Mauv",
        "job": "Account Executive"
    }, {
        "id": "53c3043a-2905-40a6-bf0c-656829e8b476",
        "state": "New York",
        "color": "Violet",
        "job": "Social Worker"
    }, {
        "id": "b9baf637-cf75-4d9f-9e60-aa7385388e44",
        "state": "North Carolina",
        "color": "Maroon",
        "job": "VP Accounting"
    }, {
        "id": "53043f61-ba4c-493c-a06e-1429fb88785f",
        "state": "Ohio",
        "color": "Puce",
        "job": "Safety Technician IV"
    }, {
        "id": "843f95da-e182-4a84-a36a-65afb8db6a46",
        "state": "Florida",
        "color": "Blue",
        "job": "Administrative Assistant III"
    }, {
        "id": "c7e38aca-4fdd-4fda-a836-79f6943d3f36",
        "state": "California",
        "color": "Khaki",
        "job": "Account Executive"
    }, {
        "id": "38931d7e-424c-48f8-98e2-8878d320bf27",
        "state": "Pennsylvania",
        "color": "Khaki",
        "job": "Recruiting Manager"
    }, {
        "id": "e375aa59-3b31-4d31-886b-e16e2c09528a",
        "state": "Pennsylvania",
        "color": "Crimson",
        "job": "Legal Assistant"
    }, {
        "id": "7ca35ce3-6085-4673-b9a6-3b9e01086285",
        "state": "Kansas",
        "color": "Fuscia",
        "job": "Chief Design Engineer"
    }, {
        "id": "bc8b46f8-26d9-4b45-875e-6984fb0c952f",
        "state": "Nevada",
        "color": "Pink",
        "job": "Legal Assistant"
    }, {
        "id": "1100452a-f03c-4374-a49b-79893649088d",
        "state": "New York",
        "color": "Maroon",
        "job": "Librarian"
    }, {
        "id": "1eb1169f-0a42-4b41-bc96-e8e411ef7cc7",
        "state": "Florida",
        "color": "Purple",
        "job": "Systems Administrator I"
    }, {
        "id": "c56c929d-ac7a-403f-aab8-64942e137a84",
        "state": "Pennsylvania",
        "color": "Red",
        "job": "Research Nurse"
    }
]

function useAnyKeyToRender() {
    const [, forceRender] = useState();

    useEffect(() => {
        window.addEventListener("keypress", forceRender);
        return () => window.removeEventListener("keypress", forceRender);
    }, [])
}

function Thing() {
    const str = "delta,echo,foxtrot"
    const words = useMemo(() => str.split(','), [str]);

    useAnyKeyToRender();
    useEffect(() => {
        console.info("XXXXXA128", words);
    }, [words])

    return (
        <strong>{words}</strong>
    )
}

const root = ReactDOM.createRoot(document.getElementById('root'));

root.render(
  <React.StrictMode>
    <App/>
  </React.StrictMode>
);

// If you want to start measuring performance in your app, pass a function
// to log results (for example: reportWebVitals(console.log))
// or send to an analytics endpoint. Learn more: https://bit.ly/CRA-vitals
reportWebVitals();
