import React, { useRef, useEffect, useState } from 'react';
import ShipDetails from './ShipDetails';

const BOAT_SIZE = 50;
const BOAT_SIZE_HALF = BOAT_SIZE / 2;

function CanvasComponent() {
  const intervalId = useRef(null);
  const canvasRef = useRef(null);
  const [imageMemory, setImageMemory] = useState(undefined);
  const [boatSummary, setBoatSummary] = useState(undefined);

  const [isOpen, setIsOpen] = useState(false);
  const [selectedId, setSelectedId] = useState(undefined);
  
  // Load images.
  useEffect(() => {
    intervalId.current = setInterval(() => {
      retrieveBoats()
    }, 5000);

    retrieveBoats()
    const bg = new Image();
    bg.src = './public/sea_map.jpg';
    bg.onload = () => {
      console.log('Background loaded');
    };

    const boatLeft = new Image();
    boatLeft.src = './public/boat3.png';
    boatLeft.onload = () => {
      console.log('Boat left loaded');
    };

    const boatRight = new Image();
    boatRight.src = './public/boat4.png';
    boatRight.onload = () => {
      console.log('Boat right loaded');
    };

    setImageMemory({
      bg,
      boatLeft,
      boatRight
    });
  }, []);

  useEffect(() => {
    return () => {
      clearInterval(intervalId.current);
    }
  }, []);

  useEffect(() => {renderCanvas()}, [boatSummary]);

  useEffect(() => {
    renderCanvas();
  }, [imageMemory]);

  const retrieveBoats = async () => {
    const response = await fetch("bff/boats/summary");
    console.log(response);

    if (response.status != 200) {
      setBoatSummary({ ships: [] });
      return;
    }

    const boatSummaryTemp = await response.json();
    console.log(boatSummaryTemp);

    setBoatSummary(boatSummaryTemp);
  }
  
  const renderCanvas = async () => {
    if (!imageMemory 
      || !imageMemory.bg 
      || !imageMemory.bg.complete 
      || !imageMemory.boatLeft.complete) return;
    
    const canvas = canvasRef.current;
    const ctx = canvas.getContext('2d');

    ctx.drawImage(imageMemory.bg, 0, 0);

    boatSummary.ships.forEach(boat => {
      boat.line.forEach(coord => {
        ctx.beginPath();
        ctx.arc(coord.x, coord.y, 6, 0, 2 * Math.PI, false);
        ctx.fillStyle = '#AF2655';
        ctx.fill();
        ctx.lineWidth = 2;
        ctx.strokeStyle = '#860A35';
        ctx.stroke();
      })

      if (boat.heading < 0)
        ctx.drawImage(imageMemory.boatLeft, boat.x - BOAT_SIZE_HALF, boat.y - BOAT_SIZE_HALF, BOAT_SIZE, BOAT_SIZE);
      else
        ctx.drawImage(imageMemory.boatRight, boat.x - BOAT_SIZE_HALF, boat.y - BOAT_SIZE_HALF, BOAT_SIZE, BOAT_SIZE);
    });

  };

  const handleClick = (event) => {
    const canvas = canvasRef.current;
    const rect = canvas.getBoundingClientRect();
    const x = event.clientX - rect.left;
    const y = event.clientY - rect.top;

    boatSummary.ships.forEach(ship => {
      if (calculateDistance(x, y, ship.x, ship.y) < 60) {
        setSelectedId(ship.id);
        setIsOpen(true);
      }
    });
  }

  const handleClose = () => {
    setIsOpen(false);
  }

  const calculateDistance = (x1, y1, x2, y2) => {
    return Math.abs(x1 - x2) + Math.abs(y1 - y2);
  }

  return (
    <>
      { boatSummary && boatSummary.ships.length == 0 && <div>No active ships found</div>}
      { imageMemory && imageMemory.bg && !imageMemory.bg.complete && <div>Loading map...</div>}
      <div>
        <canvas ref={canvasRef} width={2048} height={1536} onClick={handleClick} />
        {isOpen && 
          <ShipDetails onClose={() => handleClose()} id={selectedId}></ShipDetails> }
      </div>
    </>
  );
}

export default CanvasComponent;
