Regions
============
GIL files are compiled to GenBank .gb files. To specify a feature in the output file, use
``#region RegionName``. To end a region, use ``#endRegion``. 

Example:

.. code-block:: none

   #region Chicken Ovalbumin
   AminoSequence
   {    //The indentation isn't necessary, but it makes this more readable
       MGSIGAASMEFCFDVFKELKVHHANENIFYCPIAIMSALAMVYLGAKDSTRTQINKVVRF
       DKLPGFGDSIEAQCGTSVNVHSSLRDILNQITKPNDVYSFSLASRLYAEERYPILPEYLQ
       CVKELYRGGLEPINFQTAADQARELINSWVESQTNGIIRNVLQPSSVDSQTAMVLVNAIV
       FKGLWEKAFKDEDTQAMPFRVTEQESKPVQMMYQIGLFRVASMASEKMKILELPFASGTM
       SMLVLLPDEVSGLEQLESIINFEKLTEWTSSNVMEERKIKVYLPRMKMEEKYNLTSVLMA
       MGITDVFSSSANLSGISSAESLKISQAVHAAHAEINEAGREVVGSAEAGVDAASVSEEFR
       ADHPFLFCIKHIATNAVLFFGRCVSP
   }
   #EndRegion

Capitalization
--------------
GIL recognizes #region, #Region, #endRegion and #EndRegion. 

Nesting
-------
GIL 0.2 supports nested regions, and GIL 0.3 will support overlapping regions. 
As of GIL 0.2, #endRegion ends the last region created