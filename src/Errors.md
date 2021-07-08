

This document contains all the errors and in which situations they apply. Each one also has an associated
error code in the format "error GILxx" where x is the error number. For example, error GIL01

Errors:
=======
* Error GIL01: No region to end

You've either tried to end a region (#EndRegion) before any were created, or you tried to end a region that is closed by name later in your project (#EndRegion region name).

* Error GIL02: Region "Region name" does not exist

This error is caused when you try to end a region by name that does not exist. For example, you might have created a region with the name "TestRegion", but you tried to close the region "TesRegion"

* Error GIL03: No GIL project (.gil) in current directory

Gil compile couldn't find a .gil file

* Error GIL04: Project template "Template" not found
* Error GIL05: End token ("}") expected
* Error GIL06: The name "Name" does not exist

You referenced a sequence or operation that does not exist. If you're referencing a sequence inside another sequence or an operation inside another operation, make sure that the sequence or operation was already declaired