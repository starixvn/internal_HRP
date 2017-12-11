$("form").submit(function( event ) {
    event.preventDefault();
    var allInputs = $( "input").length;
    var filledInputs = 0;
    $('input').each(function(index, element) {
        var inputValue = $(element).val();
        if (!inputValue) {
            $(this).parent().effect('shake');
        } else {
            filledInputs++;
        }
    });

    if (allInputs == filledInputs) {
        event.currentTarget.submit();
    }
  });