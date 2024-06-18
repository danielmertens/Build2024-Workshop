import { useEffect, useState } from 'react';
import './ShipDetails.css';

function ShipDetails({ onClose, id }) {
    const [details, setDetails] = useState(undefined);

    useEffect(() => {
        if (!id) return;

        const fetchDetails = async () => {
            try {
                const response = await fetch(`bff/boats/${id}`);
                if (!response.ok) {
                    throw new Error('Network response was not ok');
                }
                const detailsJson = await response.json();
                setDetails(detailsJson);
            } catch (error) {
                console.error('Fetch error:', error);
            }
        };

        fetchDetails();
    }, [id]);

    if (!details) {
        return null;
    }

    return (
        <div className="modal-overlay">
            <div className="modal">
                <button className="modal-close" onClick={onClose}>×</button>
                <div className="modal-content">
                    <h2>{details.name}</h2>
                    <div>{details.type}</div>
                    <div>{details.ownership[0].captain}</div>
                    <br></br>
                    <div>{details.buildYear}</div>
                    <div>{details.speed} mph</div>
                    <div>Manuverability score: {details.maneuverability}</div>
                </div>
            </div>
        </div>
    );
}

export default ShipDetails;
