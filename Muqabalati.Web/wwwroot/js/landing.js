// accordian
function togglecustomaAccordion(element) {
    console.log("custom-accordion clicked");
    let content = element.nextElementSibling;
    let icon = element.querySelector(".icon");
    let isOpen = content.classList.contains("open");

    // Close all custom-accordions
    document.querySelectorAll(".custom-accordion-content").forEach((el) => {
        el.classList.remove("open");
        el.previousElementSibling.querySelector(".icon").textContent = "+";
    });

    // Toggle clicked one
    if (!isOpen) {
        content.classList.add("open");
        icon.textContent = "−";
    }
}

// Steps for interview
gsap.registerPlugin(ScrollTrigger);

// Scroll animation when section comes into view
gsap.fromTo(
    ".steps",
    { opacity: 0, y: 50 },
    {
        opacity: 1,
        y: 0,
        duration: 1,
        ease: "power3.out",
        scrollTrigger: {
            trigger: "#steps-section",
            start: "top 80%",
            toggleActions: "play none none none",
        },
    }
);

gsap.fromTo(
    ".step",
    { opacity: 0, scale: 0.8 },
    {
        opacity: 1,
        scale: 1,
        duration: 1,
        stagger: 0.3,
        ease: "back.out(1.5)",
        scrollTrigger: {
            trigger: "#steps-section",
            start: "top 80%",
            toggleActions: "play none none none",
        },
    }
);

// Hover animations
document.querySelectorAll(".step").forEach((step) => {
    step.addEventListener("mouseenter", () => {
        gsap.to(step, {
            scale: 1.1,
            duration: 0.3,
        });
        gsap.to(step.querySelector(".icon"), {
            y: -5,
            duration: 0.3,
            ease: "power1.out",
        });
    });

    step.addEventListener("mouseleave", () => {
        gsap.to(step, {
            scale: 1,
            boxShadow: "0px 0px 0px rgba(255, 255, 255, 0)",
            duration: 0.3,
        });
        gsap.to(step.querySelector(".icon"), {
            y: 0,
            duration: 0.3,
            ease: "power1.in",
        });
    });
});

// Floating effect for continuous smooth movement
gsap.to(".step", {
    y: -10,
    repeat: -1,
    yoyo: true,
    duration: 1,
    ease: "sine.inOut",
});

//  Services
gsap.fromTo(
    "#our-services",
    { opacity: 0, y: 50 },
    {
        opacity: 1,
        y: 0,
        duration: 1,
        ease: "power3.out",
        scrollTrigger: {
            trigger: "#our-services",
            start: "top 80%",
            toggleActions: "play none none none",
        },
    }
);

gsap.fromTo(
    ".service",
    { opacity: 0, scale: 0.8 },
    {
        opacity: 1,
        scale: 1,
        duration: 1,
        stagger: 0.3,
        ease: "back.out(1.5)",
        scrollTrigger: {
            trigger: "#our-services",
            start: "top 80%",
            toggleActions: "play none none none",
        },
    }
);

// Hover effects
document.querySelectorAll(".service").forEach((service) => {
    service.addEventListener("mouseenter", () => {
        gsap.to(service, {
            scale: 1.1,
            duration: 0.1,
        });

    });

    service.addEventListener("mouseleave", () => {
        gsap.to(service, {
            scale: 1,
            duration: 0.1,
        });

    });
});

// Floating effect for continuous smooth movement
gsap.to(".step", {
    y: -10,
    repeat: -1,
    yoyo: true,
    duration: 1,
    ease: "sine.inOut",
});

// Testimonials
document.addEventListener("DOMContentLoaded", function () {
    var swiper = new Swiper(".testimonials-slider", {
        loop: true,
        autoplay: {
            delay: 2000,
            disableOnInteraction: false,
        },
        spaceBetween: 20,
        grabCursor: true,
        pagination: {
            el: ".swiper-pagination",
            clickable: true,
        },
        breakpoints: {
            640: { slidesPerView: 1 },
            768: { slidesPerView: 2 },
            1024: { slidesPerView: 3 },
        },
    });
});


// Arrow up button
const backToTopBtn = document.getElementById("backToTop");
window.addEventListener("scroll", function () {
    if (window.scrollY > window.innerHeight) {
        backToTopBtn.classList.add("show");
    } else {
        backToTopBtn.classList.remove("show");
    }
});
backToTopBtn.addEventListener("click", function () {
    backToTopBtn.classList.add("rocket");

    setTimeout(() => {
        window.scrollTo({ top: 0, behavior: "smooth" });
        setTimeout(() => {
            backToTopBtn.classList.remove("rocket");
        }, 1000);
    }, 400);
});

