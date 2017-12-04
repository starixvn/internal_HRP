$("form").submit(function( event ) {
    event.preventDefault();
    var allInputs = $( "input").length;
    var filledInput = 0;
    $('input').each(function(index, element) {
        var inputValue = $(element).val();
        var check;
        if (!inputValue) {
            $(this).parent().effect('shake');
        } else {
            filledInput++;
        }
    });

    if (allInputs == filledInput) {
        event.currentTarget.submit();
    }
  });