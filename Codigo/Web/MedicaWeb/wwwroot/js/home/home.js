function setTheme(theme) {
    document.documentElement.setAttribute('data-theme', theme);
    localStorage.setItem('medica-theme', theme);

    const lightBtn = document.getElementById('lightBtn');
    const darkBtn = document.getElementById('darkBtn');

    if (lightBtn && darkBtn) {
        lightBtn.classList.toggle('active', theme === 'light');
        darkBtn.classList.toggle('active', theme === 'dark');
    }
}


document.addEventListener("DOMContentLoaded", function () {
    const sidebar = document.getElementById("sidebar");
    const main = document.querySelector(".main-wrapper");
    const toggleBtn = document.getElementById("toggleSidebar");

    const savedTheme = localStorage.getItem('medica-theme') || 'light';
    setTheme(savedTheme);

    function applySidebarState() {
        if (window.innerWidth > 780) {
            sidebar.classList.remove("collapsed");
            main.classList.remove("expanded");

            if (toggleBtn) {
                toggleBtn.style.display = 'none';
            }
        }
        else {
            if (toggleBtn) {
                toggleBtn.style.display = 'flex';
            }

            sidebar.classList.add("collapsed");
            main.classList.add("expanded");
        }
    }

    if (toggleBtn) {
        toggleBtn.addEventListener("click", function () {
            sidebar.classList.toggle("collapsed");
            main.classList.toggle("expanded");

            const isCollapsed = sidebar.classList.contains("collapsed");
            localStorage.setItem("sidebar-collapsed", isCollapsed);
        });
    }

    applySidebarState();
    window.addEventListener('resize', applySidebarState);
});