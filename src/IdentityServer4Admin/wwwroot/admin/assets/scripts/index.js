/**
 * Created by Karl on 17/07/2019.
 */

document.addEventListener("appready", handleAppReady);

function handleAppReady(e) {
    var preLoader = document.getElementById('pre-loader');
    if (preLoader != null) {
        preLoader.classList.add('tw-opacity-0');
        setTimeout(function() {
            if(preLoader.parentNode) {
                preLoader.parentNode.removeChild(preLoader);
            }
        }, 500);
    }
}

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
