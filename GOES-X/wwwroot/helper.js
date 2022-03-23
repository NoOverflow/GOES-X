function test() {
    console.log("xd");
}

var scale = 1,
    panning = false,
    pointX = 0,
    pointY = 0,
    start = { x: 0, y: 0 },
    zoom;

LockPointer = () => {
    this.requestPointerLock =
        this.requestPointerLock ||
        this.mozRequestPointerLock ||
        this.webkitPointerLockElement;
    console.log("Locking pointer.");
    zoom.requestPointerLock();
};

ReleasePointer = () => {
    document.exitPointerLock =
        document.exitPointerLock ||
        document.mozExitPointerLock ||
        document.webkitExitPointerLock;
    console.log("Releasing pointer.");
    document.exitPointerLock();
};


function setTransform() {
    zoom.style.transform = "translate(" + pointX + "px, " + pointY + "px) scale(" + scale + ")";
}

InitControls = () => {
    console.log("Initializing controls");

    zoom = document.getElementById("visualizer-content")
    
    if (zoom.children[0].localName == 'p')
        return;
    console.log("Visualizer ", zoom);

    zoom.onmousedown = function (e) {
        console.log("omd");
        e.preventDefault();
        start = { x: e.clientX - pointX, y: e.clientY - pointY };
        panning = true;
    }

    zoom.onmouseup = function (e) {
        panning = false;
    }

    zoom.onmousemove = function (e) {
        e.preventDefault();
        if (!panning) {
            return;
        }
        pointX = (e.clientX - start.x);
        pointY = (e.clientY - start.y);
        setTransform();
    }

    zoom.onwheel = function (e) {
        console.log("on_wheel");
        e.preventDefault();
        var xs = (e.clientX - pointX) / scale,
            ys = (e.clientY - pointY) / scale,
            delta = (e.wheelDelta ? e.wheelDelta : -e.deltaY);
        (delta > 0) ? (scale *= 1.2) : (scale /= 1.2);
        pointX = e.clientX - xs * scale;
        pointY = e.clientY - ys * scale;

        setTransform();
    }
};