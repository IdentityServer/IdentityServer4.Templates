var theme = {
    eventHandlers: {
        _navExpand: function() {
            theme.handleNavExpansion(this);
        },

        _toggleAll: function() {
            theme.handleToggleAll(this);
        }
    },

    init: function() {
        theme.startPrism();
        theme.initMobileNav();
        theme.expandNavIfActive();
        theme.setupNavItemEvents();
        theme.setupExpandAll();
    },

    startPrism: function() {
        if(typeof Prism != 'undefined') {
            console.info("Prism loaded");
            Prism.highlightAll();
        } else console.error("Failed to load Prism.js");
    },

    initMobileNav: function() {
        var menu = document.querySelector('header nav#nav-menu');
        var toggle = document.querySelector('header #nav-link');

        toggle.addEventListener('click', function() {
            var icon = this.querySelector('.fas');

            if(icon.classList.contains('fa-bars')) {
                icon.classList.remove('fa-bars');
                icon.classList.add('fa-times');
            } else {
                icon.classList.remove('fa-times');
                icon.classList.add('fa-bars');
            }

            if(menu.classList.contains('hidden')) menu.classList.remove('hidden');
            else menu.classList.add('hidden');

            if(document.body.classList.contains('overflow-hidden')) document.body.classList.remove('overflow-hidden');
            else document.body.classList.add('overflow-hidden');

            if(document.body.classList.contains('fixed')) document.body.classList.remove('fixed');
            else document.body.classList.add('fixed');
        });
    },

    setupExpandAll: function() {
        var toggle = document.querySelector('.js-expand-all');
        toggle.addEventListener('click', theme.eventHandlers._toggleAll);
    },

    handleToggleAll: function(toggleAll) {
        var direction = false;

        if(toggleAll.classList.contains('fa-plus-square')) {
            direction = true;
            toggleAll.classList.remove('fa-plus-square');
            toggleAll.classList.add('fa-minus-square');
        } else {
            toggleAll.classList.add('fa-plus-square');
            toggleAll.classList.remove('fa-minus-square');
        }

        var toggles = document.querySelectorAll('.js-toggle');
        for(var i = 0; i < toggles.length; i++) {
            var icon = toggles[i].querySelector('.js-icon');
            if(icon.classList.contains('fa-minus') && direction){
                theme.handleNavExpansion(toggles[i]);
            }
            theme.handleNavExpansion(toggles[i]);
        }

        if(!direction) theme.expandNavIfActive();
    },

    setupNavItemEvents: function() {
        var itemsWithChildren = document.querySelectorAll('.js-has-children');
        
        for(i = 0; i < itemsWithChildren.length; i++) {
            var toggle = itemsWithChildren[i].querySelector('.js-toggle');
            toggle.addEventListener('click', theme.eventHandlers._navExpand);
        }
    },

    handleNavExpansion: function(el) {
        var x = el.parentElement.querySelector('.js-children');
        var icon = el.querySelector('.js-icon');
        
        if(x.classList.contains('hidden')) {
            x.classList.remove('hidden');
            icon.classList.remove('fa-plus');
            icon.classList.add('fa-minus');
        } else {
            x.classList.add('hidden');
            icon.classList.remove('fa-minus');
            icon.classList.add('fa-plus');
        }
    },

    expandNavIfActive: function() {
        function toggleParent(x) {
            if(x.tagName != "LI") return;
            
            var activeToggle = x.querySelector('.js-toggle');
            if(activeToggle) theme.handleNavExpansion(activeToggle);

            var pp = x.parentElement.parentElement;
            if(pp && pp.classList.contains('js-has-children')) toggleParent(pp);
        }

        var navs = [document.querySelector('aside ul .active'), document.querySelector('nav ul .active')];
        for(i = 0; i < navs.length; i++) {
            if(navs[i]) toggleParent(navs[i].parentElement.parentElement);
        }
    }
}

window.addEventListener('load', theme.init);