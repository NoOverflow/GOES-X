function test() {
    console.log("xd");
}

var scale = 1,
    panning = false,
    pointX = 0,
    pointY = 0,
    rotAngle = 0,
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
    for (let i = 0; i < zoom.children.length; i++) {
        zoom.children[i].style.transformOrigin = "center center";
        zoom.children[i].style.transform = "rotate(" + rotAngle + "deg)";
    }
    zoom.style.transform = "translate(" + pointX + "px, " + pointY + "px) scale(" + scale + ")";
}

InitControls = () => {
    console.log("Initializing controls");

    zoom = document.getElementById("visualizer-content")

    if (zoom.children[0].localName == 'p')
        return;
    console.log("Visualizer ", zoom);

    zoom.onpointerdown = function (e) {
        console.log("omd");
        e.preventDefault();
        start = { x: e.clientX - pointX, y: e.clientY - pointY };
        panning = true;
    }

    zoom.onpointerup = function (e) {
        panning = false;
    }

    zoom.onpointerout = function (e) {
        panning = false;
    }

    zoom.onpointermove = function (e) {
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

    document.onkeyup = function (e) {
        if (!zoom || e.key != "r")
            return;
        rotAngle += 45;
        if (rotAngle > 360)
            rotAngle = 0;
        setTransform();
    }
};
