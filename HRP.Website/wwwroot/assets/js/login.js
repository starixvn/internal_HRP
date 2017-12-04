$("form").submit(function( event ) {
    event.preventDefault();
    var username = $('#Username').val();
    var pwd = $('#Password').val();
    if (!username)
    {
        $('#Username').parent().effect('shake');
    }
    if (!pwd)
    {
        $('#Password').parent().effect('shake');
    }
    if (username && pwd)
    {
        event.currentTarget.submit();
    }
  });