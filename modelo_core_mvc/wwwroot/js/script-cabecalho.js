var kebab = document.querySelector('.govsph-kebab'),
    middle = document.querySelector('.govsph-middle'),
    cross = document.querySelector('.govsph-cross'),
    dropdown = document.querySelector('.govsph-dropdown');

if(kebab){
  kebab.addEventListener('click', function() {
  middle.classList.toggle('govsph-active');
  cross.classList.toggle('govsph-active');
  dropdown.classList.toggle('govsph-active');
  kebab.classList.toggle('govsph-active');
})
}