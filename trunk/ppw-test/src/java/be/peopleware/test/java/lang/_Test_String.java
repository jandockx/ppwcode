package be.peopleware.test.java.lang;


import be.peopleware.test.CaseProvider;
import java.util.Set;
import java.util.HashSet;


/**
 * @author    Jan Dockx
 * @author    PeopleWare n.v.
 */
public class _Test_String extends CaseProvider {

  /*<section name="Meta Information">*/
  //------------------------------------------------------------------

  /** {@value} */
  public static final String CVS_REVISION = "$Revision$"; //$NON-NLS-1$
  /** {@value} */
  public static final String CVS_DATE = "$Date$"; //$NON-NLS-1$
  /** {@value} */
  public static final String CVS_STATE = "$State$"; //$NON-NLS-1$
  /** {@value} */
  public static final String CVS_TAG = "$Name$"; //$NON-NLS-1$

  /*</section>*/


  public Set getCases() {
    Set result = new HashSet();
    result.add(""); //$NON-NLS-1$
    result.add("J"); //$NON-NLS-1$
    result.add("Jan"); //$NON-NLS-1$
    result.add("JanD"); //$NON-NLS-1$
    result.add("Jan Dockx"); //$NON-NLS-1$
    result.add(" JanD"); //$NON-NLS-1$
    result.add("JanD "); //$NON-NLS-1$
    result.add(" Jan Dockx"); //$NON-NLS-1$
    result.add("Jan Dockx "); //$NON-NLS-1$
    result.add(" Jan Dockx "); //$NON-NLS-1$
    result.add(" JanD "); //$NON-NLS-1$
    result.add("this is a test sentence with more then 1024 characters" //$NON-NLS-1$
             + "*0 12345678901234567890123456789012345678901234567890" //$NON-NLS-1$
             +    "12345678901234567890123456789012345678901234567890" //$NON-NLS-1$
             + "*1 12345678901234567890123456789012345678901234567890" //$NON-NLS-1$
             +    "12345678901234567890123456789012345678901234567890" //$NON-NLS-1$
             + "*2 12345678901234567890123456789012345678901234567890" //$NON-NLS-1$
             +    "12345678901234567890123456789012345678901234567890" //$NON-NLS-1$
             + "*3 12345678901234567890123456789012345678901234567890" //$NON-NLS-1$
             +    "12345678901234567890123456789012345678901234567890" //$NON-NLS-1$
             + "*4 12345678901234567890123456789012345678901234567890" //$NON-NLS-1$
             +    "12345678901234567890123456789012345678901234567890" //$NON-NLS-1$
             + "*5 12345678901234567890123456789012345678901234567890" //$NON-NLS-1$
             +    "12345678901234567890123456789012345678901234567890" //$NON-NLS-1$
             + "*6 12345678901234567890123456789012345678901234567890" //$NON-NLS-1$
             +    "12345678901234567890123456789012345678901234567890" //$NON-NLS-1$
             + "*7 12345678901234567890123456789012345678901234567890" //$NON-NLS-1$
             +    "12345678901234567890123456789012345678901234567890" //$NON-NLS-1$
             + "*8 12345678901234567890123456789012345678901234567890" //$NON-NLS-1$
             +    "12345678901234567890123456789012345678901234567890" //$NON-NLS-1$
             + "*9 12345678901234567890123456789012345678901234567890" //$NON-NLS-1$
             +    "12345678901234567890123456789012345678901234567890" //$NON-NLS-1$
             );
    return result;
  }

  public Set getSomeCases() {
    Set result = new HashSet();
    result.add(""); //$NON-NLS-1$
    result.add("Jan Dockx"); //$NON-NLS-1$
    result.add("this is a test sentence with more then 1024 characters" //$NON-NLS-1$
             + "*0 12345678901234567890123456789012345678901234567890" //$NON-NLS-1$
             +    "12345678901234567890123456789012345678901234567890" //$NON-NLS-1$
             + "*1 12345678901234567890123456789012345678901234567890" //$NON-NLS-1$
             +    "12345678901234567890123456789012345678901234567890" //$NON-NLS-1$
             + "*2 12345678901234567890123456789012345678901234567890" //$NON-NLS-1$
             +    "12345678901234567890123456789012345678901234567890" //$NON-NLS-1$
             + "*3 12345678901234567890123456789012345678901234567890" //$NON-NLS-1$
             +    "12345678901234567890123456789012345678901234567890" //$NON-NLS-1$
             + "*4 12345678901234567890123456789012345678901234567890" //$NON-NLS-1$
             +    "12345678901234567890123456789012345678901234567890" //$NON-NLS-1$
             + "*5 12345678901234567890123456789012345678901234567890" //$NON-NLS-1$
             +    "12345678901234567890123456789012345678901234567890" //$NON-NLS-1$
             + "*6 12345678901234567890123456789012345678901234567890" //$NON-NLS-1$
             +    "12345678901234567890123456789012345678901234567890" //$NON-NLS-1$
             + "*7 12345678901234567890123456789012345678901234567890" //$NON-NLS-1$
             +    "12345678901234567890123456789012345678901234567890" //$NON-NLS-1$
             + "*8 12345678901234567890123456789012345678901234567890" //$NON-NLS-1$
             +    "12345678901234567890123456789012345678901234567890" //$NON-NLS-1$
             + "*9 12345678901234567890123456789012345678901234567890" //$NON-NLS-1$
             +    "12345678901234567890123456789012345678901234567890" //$NON-NLS-1$
             );
    return result;
  }

}
