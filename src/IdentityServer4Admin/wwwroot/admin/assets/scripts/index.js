/**
 * Created by Karl on 17/07/2019.
 */

document.addEventListener("appready", handleAppReady);

function handleAppReady(e) {
    var preBootstrapContainer = document.getElementById("pre-bootstrap-container");
    var preBootstrap = document.getElementById("pre-bootstrap");

    if (preBootstrap != null) {
        preBootstrap.className = "loaded";

        setTimeout(
            function removeLoadingScreen() {
                if (preBootstrapContainer.parentNode) {
                    preBootstrapContainer
                        .parentNode
                        .removeChild(preBootstrapContainer);
                }
            }, 500
        );
    }
}

var counter = 0;
var dots = window.setInterval(function () {
    counter++;
    var wait = document.getElementById("wait");
    if (wait) {
        if (Math.random() < .7) wait.innerHTML += ".";
        if (counter > 50) clearInterval(dots);
    } else clearInterval(dots);
}, 50);

(function() {
    var b = document.body;
    // keyboard
    b.addEventListener('keydown', function() {
        b.classList.remove('no-focus-outline');
    });
    // mouse
    b.addEventListener('mousedown', function() {
        b.classList.add('no-focus-outline');
    });
})();
